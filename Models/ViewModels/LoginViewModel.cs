using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su usuario o correo.")]
    [Display(Name = "Usuario o correo")]
    public string UsuarioOCorreo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Display(Name = "Recordarme en este equipo")]
    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}
