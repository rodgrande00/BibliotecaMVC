using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers;

public class CategoriasController : Controller
{
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
    {
        _categoriaService = categoriaService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Inicio de la consulta del listado de categorías.");
        var categorias = _categoriaService.ListarCategorias().ToList();
        _logger.LogInformation("Consulta de categorías finalizada. Total: {Total}.", categorias.Count);
        return View(categorias);
    }

    public IActionResult Details(int id)
    {
        _logger.LogInformation("Inicio de la consulta de la categoría {CategoriaId}.", id);
        var categoria = _categoriaService.ObtenerCategoria(id);
        if (categoria is null) return CategoriaNoEncontrada(id, "consultar");

        return View(categoria);
    }

    public IActionResult Create()
    {
        _logger.LogInformation("Inicio del formulario para agregar una categoría.");
        return View(new Categoria());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Categoria categoria)
    {
        _logger.LogInformation("Inicio del registro de la categoría {Nombre}.", categoria.Nombre);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("No se registró la categoría porque los datos no son válidos.");
            return View(categoria);
        }

        _categoriaService.RegistrarCategoria(categoria);
        _logger.LogInformation("Categoría {Nombre} registrada con id {CategoriaId}.", categoria.Nombre, categoria.Id);
        TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" agregada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        _logger.LogInformation("Inicio de la edición de la categoría {CategoriaId}.", id);
        var categoria = _categoriaService.ObtenerCategoria(id);
        if (categoria is null) return CategoriaNoEncontrada(id, "editar");

        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Categoria categoria)
    {
        _logger.LogInformation("Inicio del guardado de cambios de la categoría {CategoriaId}.", categoria.Id);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("No se actualizó la categoría {CategoriaId} porque los datos no son válidos.", categoria.Id);
            return View(categoria);
        }

        if (_categoriaService.ObtenerCategoria(categoria.Id) is null)
            return CategoriaNoEncontrada(categoria.Id, "guardar la edición");

        _categoriaService.ActualizarCategoria(categoria);
        _logger.LogInformation("Categoría {CategoriaId} actualizada.", categoria.Id);
        TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" actualizada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        _logger.LogInformation("Inicio de la confirmación para eliminar la categoría {CategoriaId}.", id);
        var categoria = _categoriaService.ObtenerCategoria(id);
        if (categoria is null) return CategoriaNoEncontrada(id, "eliminar");

        return View(categoria);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _logger.LogInformation("Inicio de la eliminación de la categoría {CategoriaId}.", id);
        var categoria = _categoriaService.ObtenerCategoria(id);
        if (categoria is null) return CategoriaNoEncontrada(id, "eliminar");

        _categoriaService.EliminarCategoria(id);
        _logger.LogInformation("Categoría {CategoriaId} eliminada.", id);
        TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" eliminada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private IActionResult CategoriaNoEncontrada(int id, string operacion)
    {
        _logger.LogWarning("No se encontró la categoría {CategoriaId} al {Operacion}.", id, operacion);
        return NotFound();
    }
}
