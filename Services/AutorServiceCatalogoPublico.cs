using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;

namespace BibliotecaMVC.Services;

public class AutorServiceCatalogoPublico : IAutorService
{
    private readonly IAutorRepository _autorRepository;

    public AutorServiceCatalogoPublico(IAutorRepository autorRepository)
    {
        _autorRepository = autorRepository;
    }

    public IEnumerable<Autor> ListarAutores() =>
        _autorRepository.ObtenerTodos().Where(EsVisible).ToList();

    public Autor? ObtenerAutor(int id)
    {
        var autor = _autorRepository.ObtenerPorId(id);
        if (autor is null) return null;

        return EsVisible(autor) ? autor : null;
    }

    public void RegistrarAutor(Autor autor) =>
        _autorRepository.Agregar(autor);

    public void ActualizarAutor(Autor autor)
    {
        if (ObtenerAutor(autor.Id) is null) return;

        _autorRepository.Actualizar(autor);
    }

    public void EliminarAutor(int id)
    {
        if (ObtenerAutor(id) is null) return;

        _autorRepository.Eliminar(id);
    }

    private static bool EsVisible(Autor autor) => autor.Activo;
}
