using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BibliotecaMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaLibros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    ImagenNombre = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "Id", "AnioPublicacion", "AutorId", "Disponible", "Genero", "ImagenNombre", "Isbn", "Titulo" },
                values: new object[,]
                {
                    { 1, 1967, 1, true, "Realismo mágico", "", "978-84-376-0494-7", "Cien años de soledad" },
                    { 2, 1982, 2, true, "Novela", "", "978-84-01-33723-6", "La casa de los espíritus" },
                    { 3, 1963, 3, false, "Novela", "", "978-84-204-2764-3", "La ciudad y los perros" },
                    { 4, 1944, 4, true, "Cuento", "", "978-84-206-3313-6", "Ficciones" },
                    { 5, 1963, 5, false, "Novela", "", "978-84-376-0111-3", "Rayuela" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Libros_Isbn",
                table: "Libros",
                column: "Isbn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Libros");
        }
    }
}
