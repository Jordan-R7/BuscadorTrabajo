using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuscadorTrabajo.API.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Desarrolladores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desarrolladores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NIT = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SitioWeb = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Habilidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habilidades_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerfilesProfesionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDesarrollador = table.Column<int>(type: "int", nullable: false),
                    TituloProfesional = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Modalidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GitHub = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LinkedIn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilesProfesionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilesProfesionales_Desarrolladores_IdDesarrollador",
                        column: x => x.IdDesarrollador,
                        principalTable: "Desarrolladores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEmpresa = table.Column<int>(type: "int", nullable: true),
                    IdCliente = table.Column<int>(type: "int", nullable: true),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaPublicacion = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaVencimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    Presupuesto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Salario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Moneda = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NivelExperiencia = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    TipoContrato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModalidadTrabajo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Plazo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Empresas_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPerfilProfesional = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaEmision = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaVencimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    UrlCertificado = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificaciones_PerfilesProfesionales_IdPerfilProfesional",
                        column: x => x.IdPerfilProfesional,
                        principalTable: "PerfilesProfesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Educaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPerfilProfesional = table.Column<int>(type: "int", nullable: false),
                    NivelEducativo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true),
                    EnCurso = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Educaciones_PerfilesProfesionales_IdPerfilProfesional",
                        column: x => x.IdPerfilProfesional,
                        principalTable: "PerfilesProfesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experiencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPerfilProfesional = table.Column<int>(type: "int", nullable: false),
                    Empresa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true),
                    EstadoActual = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experiencias_PerfilesProfesionales_IdPerfilProfesional",
                        column: x => x.IdPerfilProfesional,
                        principalTable: "PerfilesProfesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles_Habilidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPerfilProfesional = table.Column<int>(type: "int", nullable: false),
                    IdHabilidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles_Habilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Perfiles_Habilidades_Habilidades_IdHabilidad",
                        column: x => x.IdHabilidad,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Perfiles_Habilidades_PerfilesProfesionales_IdPerfilProfesional",
                        column: x => x.IdPerfilProfesional,
                        principalTable: "PerfilesProfesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aplicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPublicacion = table.Column<int>(type: "int", nullable: false),
                    IdDesarrollador = table.Column<int>(type: "int", nullable: false),
                    FechaAplicacion = table.Column<DateOnly>(type: "date", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlazoEstimado = table.Column<int>(type: "int", nullable: true),
                    MontoPropuesto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aplicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aplicaciones_Desarrolladores_IdDesarrollador",
                        column: x => x.IdDesarrollador,
                        principalTable: "Desarrolladores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aplicaciones_Publicaciones_IdPublicacion",
                        column: x => x.IdPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones_Habilidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPublicacion = table.Column<int>(type: "int", nullable: false),
                    IdHabilidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones_Habilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Habilidades_Habilidades_IdHabilidad",
                        column: x => x.IdHabilidad,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Habilidades_Publicaciones_IdPublicacion",
                        column: x => x.IdPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_Cedula",
                table: "Administradores",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aplicaciones_IdDesarrollador",
                table: "Aplicaciones",
                column: "IdDesarrollador");

            migrationBuilder.CreateIndex(
                name: "IX_Aplicaciones_IdPublicacion_IdDesarrollador",
                table: "Aplicaciones",
                columns: new[] { "IdPublicacion", "IdDesarrollador" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificaciones_IdPerfilProfesional",
                table: "Certificaciones",
                column: "IdPerfilProfesional");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cedula",
                table: "Clientes",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Desarrolladores_Cedula",
                table: "Desarrolladores",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educaciones_IdPerfilProfesional",
                table: "Educaciones",
                column: "IdPerfilProfesional");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_NIT",
                table: "Empresas",
                column: "NIT",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experiencias_IdPerfilProfesional",
                table: "Experiencias",
                column: "IdPerfilProfesional");

            migrationBuilder.CreateIndex(
                name: "IX_Habilidades_IdCategoria_Nombre",
                table: "Habilidades",
                columns: new[] { "IdCategoria", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_Habilidades_IdHabilidad",
                table: "Perfiles_Habilidades",
                column: "IdHabilidad");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_Habilidades_IdPerfilProfesional_IdHabilidad",
                table: "Perfiles_Habilidades",
                columns: new[] { "IdPerfilProfesional", "IdHabilidad" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesProfesionales_IdDesarrollador",
                table: "PerfilesProfesionales",
                column: "IdDesarrollador",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_IdCategoria",
                table: "Publicaciones",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_IdCliente",
                table: "Publicaciones",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_IdEmpresa",
                table: "Publicaciones",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_Habilidades_IdHabilidad",
                table: "Publicaciones_Habilidades",
                column: "IdHabilidad");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_Habilidades_IdPublicacion_IdHabilidad",
                table: "Publicaciones_Habilidades",
                columns: new[] { "IdPublicacion", "IdHabilidad" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Aplicaciones");

            migrationBuilder.DropTable(
                name: "Certificaciones");

            migrationBuilder.DropTable(
                name: "Educaciones");

            migrationBuilder.DropTable(
                name: "Experiencias");

            migrationBuilder.DropTable(
                name: "Perfiles_Habilidades");

            migrationBuilder.DropTable(
                name: "Publicaciones_Habilidades");

            migrationBuilder.DropTable(
                name: "PerfilesProfesionales");

            migrationBuilder.DropTable(
                name: "Habilidades");

            migrationBuilder.DropTable(
                name: "Publicaciones");

            migrationBuilder.DropTable(
                name: "Desarrolladores");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
