namespace BibliotecaMVC.Seguridad;

public static class RolesBiblioteca
{
    public const string Usuario = "Usuario";
    public const string Bibliotecario = "Bibliotecario";
    public const string Admin = "Admin";

    public const string Consulta = $"{Usuario},{Bibliotecario},{Admin}";
    public const string Modificacion = $"{Bibliotecario},{Admin}";
}
