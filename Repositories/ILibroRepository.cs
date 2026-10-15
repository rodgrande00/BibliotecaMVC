using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories;

public interface ILibroRepository
{
    IEnumerable<Libro> ObtenerTodos();
    Libro? ObtenerPorId(int id);
    void Agregar(Libro libro);
    void Actualizar(Libro libro);
    void Eliminar(int id);
    bool ExisteOtroConMismoIsbn(string isbn, int idActual);
}
