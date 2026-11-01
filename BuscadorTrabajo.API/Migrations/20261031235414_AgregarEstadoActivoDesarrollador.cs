using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuscadorTrabajo.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoActivoDesarrollador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Desarrolladores",
                type: "bit",
                nullable: false,
                defaultValue: true); // Los desarrolladores que ya existen quedan activos
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Desarrolladores");
        }
    }
}
