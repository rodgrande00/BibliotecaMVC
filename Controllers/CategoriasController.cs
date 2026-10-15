using BibliotecaMVC.Models;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers;

public class CategoriasController : Controller
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    public IActionResult Index() =>
        View(_categoriaService.ListarCategorias());

    public IActionResult Details(int id)
    {
        var categoria = _categoriaService.ObtenerCategoria(id);
        return categoria is null ? NotFound() : View(categoria);
    }

    public IActionResult Create() =>
        View(new Categoria());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Categoria categoria)
    {
        if (!ModelState.IsValid) return View(categoria);

        _categoriaService.RegistrarCategoria(categoria);
        TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" agregada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var categoria = _categoriaService.ObtenerCategoria(id);
        return categoria is null ? NotFound() : View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Categoria categoria)
    {
        if (!ModelState.IsValid) return View(categoria);

        if (_categoriaService.ObtenerCategoria(categoria.Id) is null) return NotFound();

        _categoriaService.ActualizarCategoria(categoria);
        TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" actualizada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var categoria = _categoriaService.ObtenerCategoria(id);
        return categoria is null ? NotFound() : View(categoria);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var categoria = _categoriaService.ObtenerCategoria(id);
        if (categoria is null) return NotFound();

        _categoriaService.EliminarCategoria(id);
        TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" eliminada correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
