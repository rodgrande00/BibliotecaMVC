using BibliotecaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers;

public class AccountController : Controller
{
    private const string CredencialesInvalidas = "El usuario, correo o contraseña no son válidos.";

    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        if (!await IniciarSesionAsync(modelo))
        {
            ModelState.AddModelError(string.Empty, CredencialesInvalidas);
            return View(modelo);
        }

        return RedirigirTrasLogin(modelo.ReturnUrl);
    }

    [HttpGet]
    public IActionResult Registro() => View(new RegistroViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        var usuario = new IdentityUser
        {
            UserName = modelo.NombreUsuario,
            Email = modelo.Correo
        };

        var resultado = await _userManager.CreateAsync(usuario, modelo.Contrasena);
        if (!resultado.Succeeded)
        {
            AgregarErrores(resultado);
            return View(modelo);
        }

        await _signInManager.SignInAsync(usuario, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        _logger.LogWarning(
            "Acceso denegado para {Usuario} al intentar {Ruta}.",
            User.Identity?.Name ?? "anónimo",
            Request.Query["ReturnUrl"].ToString());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private async Task<bool> IniciarSesionAsync(LoginViewModel modelo)
    {
        var usuario = await _userManager.FindByEmailAsync(modelo.UsuarioOCorreo)
            ?? await _userManager.FindByNameAsync(modelo.UsuarioOCorreo);

        if (usuario?.UserName is null) return false;

        var resultado = await _signInManager.PasswordSignInAsync(
            usuario.UserName,
            modelo.Contrasena,
            modelo.Recordarme,
            lockoutOnFailure: false);

        return resultado.Succeeded;
    }

    private IActionResult RedirigirTrasLogin(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    private void AgregarErrores(IdentityResult resultado)
    {
        foreach (var error in resultado.Errors)
            ModelState.AddModelError(string.Empty, TraducirError(error));
    }

    private static string TraducirError(IdentityError error) => error.Code switch
    {
        "DuplicateUserName" => "Ese nombre de usuario ya está registrado.",
        "DuplicateEmail" => "Ese correo ya está registrado.",
        "PasswordTooShort" => "La contraseña debe tener al menos 6 caracteres.",
        "PasswordRequiresDigit" => "La contraseña debe incluir un número.",
        "PasswordRequiresUpper" => "La contraseña debe incluir una letra mayúscula.",
        "PasswordRequiresLower" => "La contraseña debe incluir una letra minúscula.",
        _ => error.Description
    };
}
