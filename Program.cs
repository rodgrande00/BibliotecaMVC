using BibliotecaMVC.Data;
using BibliotecaMVC.Repositories;
using BibliotecaMVC.Seguridad;
using BibliotecaMVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BibliotecaDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(opciones =>
    {
        opciones.User.RequireUniqueEmail = true;
        opciones.Password.RequiredLength = 6;
        opciones.Password.RequireDigit = true;
        opciones.Password.RequireUppercase = true;
        opciones.Password.RequireLowercase = true;
        opciones.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<BibliotecaDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Account/Login";
    opciones.LogoutPath = "/Account/Logout";
    opciones.AccessDeniedPath = "/Account/AccesoDenegado";
});

builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<IAutorService, AutorServiceCatalogoPublico>();
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IAlmacenamientoImagenes, AlmacenamientoImagenesLocal>();
builder.Services.AddScoped<IContenidoPortalService, ContenidoPortalService>();

var app = builder.Build();

// Crea o actualiza las tablas de EF Core (Libros e Identity). Categorías sigue
// atendiéndose con el script SQL y ADO.NET.
using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<BibliotecaDbContext>();
    contexto.Database.Migrate();
}

await InicializadorIdentidad.PrepararAsync(app.Services);

// El detalle de la excepción queda en el log. El usuario solo ve un mensaje general.
app.UseExceptionHandler("/Home/Error");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// MapStaticAssets solo sirve los archivos que existían al compilar. Las portadas
// que el usuario sube en tiempo de ejecución a wwwroot/images necesitan además
// este middleware, de lo contrario darían 404.
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
