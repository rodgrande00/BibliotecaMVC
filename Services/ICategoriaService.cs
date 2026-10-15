using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services;

public interface ICategoriaService
{
    IEnumerable<Categoria> ListarCategorias();
    Categoria? ObtenerCategoria(int id);
    void RegistrarCategoria(Categoria categoria);
    void ActualizarCategoria(Categoria categoria);
    void EliminarCategoria(int id);
}
