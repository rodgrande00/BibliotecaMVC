using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Repositories;

/// <summary>
/// Persistencia de libros con Entity Framework Core. Mostrar consulta el DbSet
/// y agregar usa Add más SaveChanges. No usa ADO.NET.
/// </summary>
public class LibroRepository : ILibroRepository
{
    private readonly BibliotecaDbContext _contexto;

    public LibroRepository(BibliotecaDbContext contexto)
    {
        _contexto = contexto;
    }

    public IEnumerable<Libro> ObtenerTodos() =>
        _contexto.Libros
            .AsNoTracking()
            .OrderBy(libro => libro.Titulo)
            .ToList();

    public Libro? ObtenerPorId(int id) =>
        _contexto.Libros.AsNoTracking().FirstOrDefault(libro => libro.Id == id);

    public void Agregar(Libro libro)
    {
        _contexto.Libros.Add(libro);
        _contexto.SaveChanges();
    }

    public void Actualizar(Libro libro)
    {
        _contexto.Libros.Update(libro);
        _contexto.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var libro = _contexto.Libros.Find(id);
        if (libro is null) return;

        _contexto.Libros.Remove(libro);
        _contexto.SaveChanges();
    }

    public bool ExisteOtroConMismoIsbn(string isbn, int idActual) =>
        _contexto.Libros.Any(libro => libro.Isbn == isbn && libro.Id != idActual);
}
