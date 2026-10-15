using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services;

public interface ILibroService
{
    IEnumerable<Libro> ListarLibros();
    Libro? ObtenerLibro(int id);
    void RegistrarLibro(Libro libro);
    void ActualizarLibro(Libro libro);
    void EliminarLibro(int id);
    bool IsbnYaRegistrado(string isbn, int idActual);
}
