namespace BibliotecaMVC.Seguridad;

public static class RolesBiblioteca
{
    public const string Lector = "Lector";
    public const string Administrador = "Administrador";

    public const string Consulta = $"{Lector},{Administrador}";
}
