using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Seguridad;

/// <summary>
/// Crea los roles de la biblioteca y asegura un usuario de cada nivel.
/// Si el usuario ya existe, solo le asigna el rol que le falta.
/// </summary>
public static class InicializadorIdentidad
{
    public static async Task PrepararAsync(IServiceProvider servicios)
    {
        using var alcance = servicios.CreateScope();
        var roles = alcance.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var usuarios = alcance.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await AsegurarRolAsync(roles, RolesBiblioteca.Lector);
        await AsegurarRolAsync(roles, RolesBiblioteca.Administrador);

        await AsegurarUsuarioAsync(
            usuarios, "lector", "lector@biblioteca.test", "Biblioteca1", RolesBiblioteca.Lector);
        await AsegurarUsuarioAsync(
            usuarios, "admin", "admin@biblioteca.test", "Biblioteca1", RolesBiblioteca.Administrador);
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
