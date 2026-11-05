using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuscadorTrabajo.API.Migrations
{
    /// <inheritdoc />
    public partial class FotoPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FotoId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Imagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TipoContenido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Datos = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Tamano = table.Column<int>(type: "int", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagenes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FotoId",
                table: "AspNetUsers",
                column: "FotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Imagenes_FotoId",
                table: "AspNetUsers",
                column: "FotoId",
                principalTable: "Imagenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Imagenes_FotoId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Imagenes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FotoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FotoId",
                table: "AspNetUsers");
        }
    }
}
