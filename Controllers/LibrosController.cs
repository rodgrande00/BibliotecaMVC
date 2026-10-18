using BibliotecaMVC.Models;
using BibliotecaMVC.Models.ViewModels;
using BibliotecaMVC.Seguridad;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibliotecaMVC.Controllers;

[Authorize(Roles = RolesBiblioteca.Consulta)]
public class LibrosController : Controller
{
    private const string SinAutor = "Autor no registrado";

    private readonly ILibroService _libroService;
    private readonly IAutorService _autorService;
    private readonly IAlmacenamientoImagenes _almacenamientoImagenes;

    public LibrosController(
        ILibroService libroService,
        IAutorService autorService,
        IAlmacenamientoImagenes almacenamientoImagenes)
    {
        _libroService = libroService;
        _autorService = autorService;
        _almacenamientoImagenes = almacenamientoImagenes;
    }

    public IActionResult Index()
    {
        var libros = _libroService.ListarLibros().Select(MapearAVistaModelo).ToList();
        return View(libros);
    }

    public IActionResult Details(int id)
    {
        var libro = _libroService.ObtenerLibro(id);
        return libro is null ? NotFound() : View(MapearAVistaModelo(libro));
    }

    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    public IActionResult Create() =>
        View(ConstruirFormulario(new Libro { Disponible = true }));

    [HttpPost]
    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LibroFormViewModel formulario)
    {
        ValidarPortada(formulario.Imagen);
        ValidarIsbnUnico(formulario.Libro);

        if (!ModelState.IsValid)
            return View(ConstruirFormulario(formulario.Libro));

        formulario.Libro.ImagenNombre = await GuardarPortadaAsync(formulario.Imagen) ?? string.Empty;
        _libroService.RegistrarLibro(formulario.Libro);
        TempData["Mensaje"] = $"Libro \"{formulario.Libro.Titulo}\" agregado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    public IActionResult Edit(int id)
    {
        var libro = _libroService.ObtenerLibro(id);
        return libro is null ? NotFound() : View(ConstruirFormulario(libro));
    }

    [HttpPost]
    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LibroFormViewModel formulario)
    {
        var libroExistente = _libroService.ObtenerLibro(formulario.Libro.Id);
        if (libroExistente is null) return NotFound();

        var portadaAnterior = libroExistente.ImagenNombre;
        ValidarPortada(formulario.Imagen);
        ValidarIsbnUnico(formulario.Libro);

        if (!ModelState.IsValid)
        {
            // Sin esto el formulario volvería mostrando la portada genérica
            // en lugar de la que el libro ya tiene guardada.
            formulario.Libro.ImagenNombre = portadaAnterior;
            return View(ConstruirFormulario(formulario.Libro));
        }

        var portadaNueva = await GuardarPortadaAsync(formulario.Imagen);
        formulario.Libro.ImagenNombre = portadaNueva ?? portadaAnterior;

        _libroService.ActualizarLibro(formulario.Libro);

        if (portadaNueva is not null)
            _almacenamientoImagenes.Eliminar(portadaAnterior);

        TempData["Mensaje"] = $"Libro \"{formulario.Libro.Titulo}\" actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    public IActionResult Delete(int id)
    {
        var libro = _libroService.ObtenerLibro(id);
        return libro is null ? NotFound() : View(MapearAVistaModelo(libro));
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var libro = _libroService.ObtenerLibro(id);
        if (libro is null) return NotFound();

        _libroService.EliminarLibro(id);
        _almacenamientoImagenes.Eliminar(libro.ImagenNombre);
        TempData["Mensaje"] = $"Libro \"{libro.Titulo}\" eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private LibroViewModel MapearAVistaModelo(Libro libro) => new()
    {
        Id = libro.Id,
        Titulo = libro.Titulo,
        Isbn = libro.Isbn,
        NombreAutor = _autorService.ObtenerAutor(libro.AutorId)?.NombreCompleto ?? SinAutor,
        AnioPublicacion = libro.AnioPublicacion,
        Genero = libro.Genero,
        Disponible = libro.Disponible,
        ImagenUrl = _almacenamientoImagenes.ObtenerUrl(libro.ImagenNombre)
    };

    private LibroFormViewModel ConstruirFormulario(Libro libro) => new()
    {
        Libro = libro,
        ImagenActualUrl = _almacenamientoImagenes.ObtenerUrl(libro.ImagenNombre),
        Autores = ObtenerOpcionesDeAutores(libro.AutorId)
    };

    private IEnumerable<SelectListItem> ObtenerOpcionesDeAutores(int autorSeleccionadoId) =>
        _autorService.ListarAutores()
            .Select(autor => new SelectListItem
            {
                Value = autor.Id.ToString(),
                Text = autor.NombreCompleto,
                Selected = autor.Id == autorSeleccionadoId
            })
            .ToList();

    private void ValidarIsbnUnico(Libro libro)
    {
        if (_libroService.IsbnYaRegistrado(libro.Isbn, libro.Id))
            ModelState.AddModelError("Libro.Isbn", "Ya existe un libro con ese ISBN.");
    }

    private void ValidarPortada(IFormFile? imagen)
    {
        if (imagen is null) return;

        var error = _almacenamientoImagenes.Validar(imagen);
        if (error is not null)
            ModelState.AddModelError(nameof(LibroFormViewModel.Imagen), error);
    }

    private async Task<string?> GuardarPortadaAsync(IFormFile? imagen) =>
        imagen is null ? null : await _almacenamientoImagenes.GuardarAsync(imagen);
}
