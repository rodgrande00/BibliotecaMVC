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
    private readonly ILogger<LibrosController> _logger;

    public LibrosController(
        ILibroService libroService,
        IAutorService autorService,
        IAlmacenamientoImagenes almacenamientoImagenes,
        ILogger<LibrosController> logger)
    {
        _libroService = libroService;
        _autorService = autorService;
        _almacenamientoImagenes = almacenamientoImagenes;
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Inicio de la consulta del listado de libros.");
        var libros = _libroService.ListarLibros().Select(MapearAVistaModelo).ToList();
        _logger.LogInformation("Consulta de libros finalizada. Total: {Total}.", libros.Count);
        return View(libros);
    }

    public IActionResult Details(int id)
    {
        _logger.LogInformation("Inicio de la consulta del libro {LibroId}.", id);
        var libro = _libroService.ObtenerLibro(id);
        if (libro is null) return LibroNoEncontrado(id, "consultar");

        return View(MapearAVistaModelo(libro));
    }

    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    public IActionResult Create()
    {
        _logger.LogInformation("Inicio del formulario para agregar un libro.");
        return View(ConstruirFormulario(new Libro { Disponible = true }));
    }

    [HttpPost]
    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LibroFormViewModel formulario)
    {
        _logger.LogInformation("Inicio del registro del libro {Titulo}.", formulario.Libro.Titulo);
        ValidarPortada(formulario.Imagen);
        ValidarIsbnUnico(formulario.Libro);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("No se registró el libro {Titulo} porque los datos no son válidos.", formulario.Libro.Titulo);
            return View(ConstruirFormulario(formulario.Libro));
        }

        formulario.Libro.ImagenNombre = await GuardarPortadaAsync(formulario.Imagen) ?? string.Empty;
        _libroService.RegistrarLibro(formulario.Libro);
        _logger.LogInformation("Libro {Titulo} registrado con id {LibroId}.", formulario.Libro.Titulo, formulario.Libro.Id);
        TempData["Mensaje"] = $"Libro \"{formulario.Libro.Titulo}\" agregado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    public IActionResult Edit(int id)
    {
        _logger.LogInformation("Inicio de la edición del libro {LibroId}.", id);
        var libro = _libroService.ObtenerLibro(id);
        if (libro is null) return LibroNoEncontrado(id, "editar");

        return View(ConstruirFormulario(libro));
    }

    [HttpPost]
    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LibroFormViewModel formulario)
    {
        _logger.LogInformation("Inicio del guardado de cambios del libro {LibroId}.", formulario.Libro.Id);
        var libroExistente = _libroService.ObtenerLibro(formulario.Libro.Id);
        if (libroExistente is null) return LibroNoEncontrado(formulario.Libro.Id, "guardar la edición");

        var portadaAnterior = libroExistente.ImagenNombre;
        ValidarPortada(formulario.Imagen);
        ValidarIsbnUnico(formulario.Libro);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("No se actualizó el libro {LibroId} porque los datos no son válidos.", formulario.Libro.Id);
            formulario.Libro.ImagenNombre = portadaAnterior;
            return View(ConstruirFormulario(formulario.Libro));
        }

        var portadaNueva = await GuardarPortadaAsync(formulario.Imagen);
        formulario.Libro.ImagenNombre = portadaNueva ?? portadaAnterior;

        _libroService.ActualizarLibro(formulario.Libro);
        _logger.LogInformation("Libro {LibroId} actualizado.", formulario.Libro.Id);

        if (portadaNueva is not null)
            _almacenamientoImagenes.Eliminar(portadaAnterior);

        TempData["Mensaje"] = $"Libro \"{formulario.Libro.Titulo}\" actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation("Inicio de la confirmación para eliminar el libro {LibroId}.", id);
        var libro = _libroService.ObtenerLibro(id);
        if (libro is null) return LibroNoEncontrado(id, "eliminar");

        return View(MapearAVistaModelo(libro));
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = RolesBiblioteca.Modificacion)]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _logger.LogInformation("Inicio de la eliminación del libro {LibroId}.", id);
        var libro = _libroService.ObtenerLibro(id);
        if (libro is null) return LibroNoEncontrado(id, "eliminar");

        _libroService.EliminarLibro(id);
        _logger.LogInformation("Libro {LibroId} eliminado.", id);
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

    private IActionResult LibroNoEncontrado(int id, string operacion)
    {
        _logger.LogWarning("No se encontró el libro {LibroId} al {Operacion}.", id, operacion);
        return NotFound();
    }

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
