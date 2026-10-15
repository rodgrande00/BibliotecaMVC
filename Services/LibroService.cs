using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;

namespace BibliotecaMVC.Services;

public class LibroService : ILibroService
{
    private readonly ILibroRepository _libroRepository;

    public LibroService(ILibroRepository libroRepository)
    {
        _libroRepository = libroRepository;
    }

    public IEnumerable<Libro> ListarLibros() =>
        _libroRepository.ObtenerTodos();

    public Libro? ObtenerLibro(int id) =>
        _libroRepository.ObtenerPorId(id);

    public void RegistrarLibro(Libro libro) =>
        _libroRepository.Agregar(libro);

    public void ActualizarLibro(Libro libro) =>
        _libroRepository.Actualizar(libro);

    public void EliminarLibro(int id) =>
        _libroRepository.Eliminar(id);

    public bool IsbnYaRegistrado(string isbn, int idActual) =>
        _libroRepository.ExisteOtroConMismoIsbn(isbn, idActual);
}
