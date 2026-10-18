using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Seguridad;

/// <summary>
/// Crea Admin, Bibliotecario y Usuario solo si no existen, y asigna
/// un rol a cada cuenta de prueba sin duplicar la asignación.
/// </summary>
public static class InicializadorIdentidad
{
    public static async Task PrepararAsync(IServiceProvider servicios)
    {
        using var alcance = servicios.CreateScope();
        var roles = alcance.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var usuarios = alcance.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await AsegurarRolAsync(roles, RolesBiblioteca.Usuario);
        await AsegurarRolAsync(roles, RolesBiblioteca.Bibliotecario);
        await AsegurarRolAsync(roles, RolesBiblioteca.Admin);

        await AsegurarUsuarioAsync(
            usuarios, "lector", "lector@biblioteca.test", "Biblioteca1", RolesBiblioteca.Usuario);
        await AsegurarUsuarioAsync(
            usuarios, "bibliotecario", "bibliotecario@biblioteca.test", "Biblioteca1", RolesBiblioteca.Bibliotecario);
        await AsegurarUsuarioAsync(
            usuarios, "admin", "admin@biblioteca.test", "Biblioteca1", RolesBiblioteca.Admin);
    }

    private static async Task AsegurarRolAsync(RoleManager<IdentityRole> roles, string nombre)
    {
        if (await roles.RoleExistsAsync(nombre)) return;

        var resultado = await roles.CreateAsync(new IdentityRole(nombre));
        if (!resultado.Succeeded)
            throw new InvalidOperationException($"No se pudo crear el rol {nombre}.");
    }

    private static async Task AsegurarUsuarioAsync(
        UserManager<IdentityUser> usuarios,
        string nombre,
        string correo,
        string contrasena,
        string rol)
    {
        var usuario = await usuarios.FindByNameAsync(nombre);
        if (usuario is null)
        {
            usuario = new IdentityUser { UserName = nombre, Email = correo };
            var creado = await usuarios.CreateAsync(usuario, contrasena);
            if (!creado.Succeeded)
                throw new InvalidOperationException($"No se pudo crear el usuario {nombre}.");
        }

        if (await usuarios.IsInRoleAsync(usuario, rol)) return;

        var asignado = await usuarios.AddToRoleAsync(usuario, rol);
        if (!asignado.Succeeded)
            throw new InvalidOperationException($"No se pudo asignar el rol {rol} a {nombre}.");
    }
}
