using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;

namespace BibliotecaMVC.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public IEnumerable<Categoria> ListarCategorias() =>
        _categoriaRepository.ObtenerTodos();

    public Categoria? ObtenerCategoria(int id) =>
        _categoriaRepository.ObtenerPorId(id);

    public void RegistrarCategoria(Categoria categoria) =>
        _categoriaRepository.Agregar(categoria);

    public void ActualizarCategoria(Categoria categoria) =>
        _categoriaRepository.Actualizar(categoria);

    public void EliminarCategoria(int id) =>
        _categoriaRepository.Eliminar(id);
}
