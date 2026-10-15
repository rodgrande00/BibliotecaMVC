using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data;

/// <summary>
/// Contexto de Entity Framework Core para SQL Server.
/// Autores sigue en memoria y Categorías usa ADO.NET, así que este contexto
/// solo modela Libros: AutorId se guarda como dato, sin relación a otra tabla.
/// </summary>
public class BibliotecaDbContext : DbContext
{
    public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Libro> Libros => Set<Libro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarLibro(modelBuilder);
    }

    private static void ConfigurarLibro(ModelBuilder modelBuilder)
    {
        var libro = modelBuilder.Entity<Libro>();

        libro.ToTable("Libros");
        libro.HasKey(item => item.Id);
        libro.Property(item => item.Titulo).IsRequired().HasMaxLength(200);
        libro.Property(item => item.Isbn).IsRequired().HasMaxLength(20);
        libro.Property(item => item.Genero).IsRequired().HasMaxLength(60);
        libro.Property(item => item.ImagenNombre).IsRequired().HasMaxLength(260);
        libro.HasIndex(item => item.Isbn).IsUnique();
        libro.HasData(LibrosIniciales());
    }

    private static Libro[] LibrosIniciales() =>
    [
        new Libro
        {
            Id = 1,
            Titulo = "Cien años de soledad",
            Isbn = "978-84-376-0494-7",
            AutorId = 1,
            AnioPublicacion = 1967,
            Genero = "Realismo mágico",
            Disponible = true
        },
        new Libro
        {
            Id = 2,
            Titulo = "La casa de los espíritus",
            Isbn = "978-84-01-33723-6",
            AutorId = 2,
            AnioPublicacion = 1982,
            Genero = "Novela",
            Disponible = true
        },
        new Libro
        {
            Id = 3,
            Titulo = "La ciudad y los perros",
            Isbn = "978-84-204-2764-3",
            AutorId = 3,
            AnioPublicacion = 1963,
            Genero = "Novela",
            Disponible = false
        },
        new Libro
        {
            Id = 4,
            Titulo = "Ficciones",
            Isbn = "978-84-206-3313-6",
            AutorId = 4,
            AnioPublicacion = 1944,
            Genero = "Cuento",
            Disponible = true
        },
        new Libro
        {
            Id = 5,
            Titulo = "Rayuela",
            Isbn = "978-84-376-0111-3",
            AutorId = 5,
            AnioPublicacion = 1963,
            Genero = "Novela",
            Disponible = false
        }
    ];
}
