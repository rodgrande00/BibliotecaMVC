using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> ObtenerTodos();
    Categoria? ObtenerPorId(int id);
    void Agregar(Categoria categoria);
    void Actualizar(Categoria categoria);
    void Eliminar(int id);
}
