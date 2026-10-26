using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Models.ViewModels;
using BibliotecaMVC.Services;

namespace BibliotecaMVC.Controllers;

public class HomeController : Controller
{
    private readonly IContenidoPortalService _contenidoPortal;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IContenidoPortalService contenidoPortal, ILogger<HomeController> logger)
    {
        _contenidoPortal = contenidoPortal;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var modelo = new InicioViewModel
        {
            Cifras = _contenidoPortal.ListarCifras(),
            Categorias = _contenidoPortal.ListarCategorias(),
            Servicios = _contenidoPortal.ListarServicios(),
            AutoresDestacados = _contenidoPortal.ListarAutoresDestacados()
        };

        return View(modelo);
    }


    public IActionResult Usuarios()
    {
        return View();
    }

    public IActionResult Prestamos()
    {
        return View();
    }

    public IActionResult AcercaDe()
    {
        var modelo = new AcercaDeViewModel
        {
            Cifras = _contenidoPortal.ListarCifras(),
            Principios = _contenidoPortal.ListarPrincipios(),
            Servicios = _contenidoPortal.ListarServicios(),
            Equipo = _contenidoPortal.ListarEquipo()
        };

        return View(modelo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var excepcion = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (excepcion?.Error is not null)
            _logger.LogError(excepcion.Error, "Error no controlado al procesar {Ruta}.", excepcion.Path);

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
