using System.Data;
using BibliotecaMVC.Models;
using Microsoft.Data.SqlClient;

namespace BibliotecaMVC.Repositories;

/// <summary>
/// Acceso a datos de Categorías con ADO.NET puro, sin Entity Framework Core.
/// Todas las consultas usan parámetros SQL: así se evita la inyección SQL y el
/// motor puede reutilizar el plan de ejecución.
/// </summary>
public class CategoriaRepository : ICategoriaRepository
{
    private const int LargoNombre = 100;
    private const int LargoDescripcion = 400;

    private readonly string _cadenaConexion;

    public CategoriaRepository(IConfiguration configuration)
    {
        _cadenaConexion = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Falta la cadena de conexión 'DefaultConnection' en appsettings.json.");
    }

    public IEnumerable<Categoria> ObtenerTodos()
    {
        const string sql = "SELECT Id, Nombre, Descripcion FROM dbo.Categorias ORDER BY Nombre;";

        var categorias = new List<Categoria>();

        using var conexion = new SqlConnection(_cadenaConexion);
        using var comando = new SqlCommand(sql, conexion);

        conexion.Open();
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            categorias.Add(LeerCategoria(lector));
        }

        return categorias;
    }

    public Categoria? ObtenerPorId(int id)
    {
        const string sql = "SELECT Id, Nombre, Descripcion FROM dbo.Categorias WHERE Id = @Id;";

        using var conexion = new SqlConnection(_cadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        conexion.Open();
        using var lector = comando.ExecuteReader();

        return lector.Read() ? LeerCategoria(lector) : null;
    }

    public void Agregar(Categoria categoria)
    {
        const string sql = """
            INSERT INTO dbo.Categorias (Nombre, Descripcion)
            VALUES (@Nombre, @Descripcion);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var conexion = new SqlConnection(_cadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        AgregarParametrosDatos(comando, categoria);

        conexion.Open();

        // SCOPE_IDENTITY devuelve el Id que asignó la base, así la entidad queda
        // consistente con lo que se acaba de guardar.
        categoria.Id = Convert.ToInt32(comando.ExecuteScalar());
    }

    public void Actualizar(Categoria categoria)
    {
        const string sql = """
            UPDATE dbo.Categorias
            SET Nombre = @Nombre,
                Descripcion = @Descripcion
            WHERE Id = @Id;
            """;

        using var conexion = new SqlConnection(_cadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        AgregarParametrosDatos(comando, categoria);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = categoria.Id;

        conexion.Open();
        comando.ExecuteNonQuery();
    }

    public void Eliminar(int id)
    {
        const string sql = "DELETE FROM dbo.Categorias WHERE Id = @Id;";

        using var conexion = new SqlConnection(_cadenaConexion);
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        conexion.Open();
        comando.ExecuteNonQuery();
    }

    private static void AgregarParametrosDatos(SqlCommand comando, Categoria categoria)
    {
        comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, LargoNombre).Value = categoria.Nombre;
        comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, LargoDescripcion).Value = categoria.Descripcion;
    }

    private static Categoria LeerCategoria(SqlDataReader lector) => new()
    {
        Id = lector.GetInt32(lector.GetOrdinal("Id")),
        Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
        Descripcion = lector.GetString(lector.GetOrdinal("Descripcion"))
    };
}
