using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictamenes.Migrations
{
    public partial class PrimeraVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivoPDF",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    path = table.Column<string>(nullable: true),
                    contenido = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivoPDF", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Cuil = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Cuil);
                });

            migrationBuilder.CreateTable(
                name: "Asunto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    descripcion = table.Column<string>(maxLength: 50, nullable: true),
                    EstaHabilitado = table.Column<bool>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionCuil = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asunto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asunto_Productos_UsuarioModificacionCuil",
                        column: x => x.UsuarioModificacionCuil,
                        principalTable: "Productos",
                        principalColumn: "Cuil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    descripcion = table.Column<string>(maxLength: 50, nullable: true),
                    EstaHabilitado = table.Column<bool>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionCuil = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Productos_UsuarioModificacionCuil",
                        column: x => x.UsuarioModificacionCuil,
                        principalTable: "Productos",
                        principalColumn: "Cuil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    descripcion = table.Column<string>(maxLength: 50, nullable: true),
                    EstaHabilitado = table.Column<bool>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionCuil = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empleados_Productos_UsuarioModificacionCuil",
                        column: x => x.UsuarioModificacionCuil,
                        principalTable: "Productos",
                        principalColumn: "Cuil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    CuilCuit = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(maxLength: 50, nullable: true),
                    RazonSocial = table.Column<string>(maxLength: 80, nullable: true),
                    IdTipoSujetoObligado = table.Column<int>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionCuil = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.CuilCuit);
                    table.ForeignKey(
                        name: "FK_Clientes_Empleados_IdTipoSujetoObligado",
                        column: x => x.IdTipoSujetoObligado,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clientes_Productos_UsuarioModificacionCuil",
                        column: x => x.UsuarioModificacionCuil,
                        principalTable: "Productos",
                        principalColumn: "Cuil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NroGDE = table.Column<string>(maxLength: 30, nullable: false),
                    NroExpediente = table.Column<string>(maxLength: 30, nullable: false),
                    FechaCarga = table.Column<DateTime>(nullable: false),
                    Detalle = table.Column<string>(nullable: true),
                    EsPublico = table.Column<bool>(nullable: false),
                    IdArchivoLigado = table.Column<int>(nullable: false),
                    ArchivoLigadoId = table.Column<int>(nullable: true),
                    IdSujetoObligado = table.Column<int>(nullable: false),
                    IdAsunto = table.Column<int>(nullable: false),
                    IdTipoDictamen = table.Column<int>(nullable: false),
                    IdUsuarioGenerador = table.Column<int>(nullable: false),
                    UsuarioGeneradorCuil = table.Column<int>(nullable: true),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionCuil = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.id);
                    table.ForeignKey(
                        name: "FK_Categorias_ArchivoPDF_ArchivoLigadoId",
                        column: x => x.ArchivoLigadoId,
                        principalTable: "ArchivoPDF",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categorias_Asunto_IdAsunto",
                        column: x => x.IdAsunto,
                        principalTable: "Asunto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Categorias_Clientes_IdSujetoObligado",
                        column: x => x.IdSujetoObligado,
                        principalTable: "Clientes",
                        principalColumn: "CuilCuit",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Categorias_Compras_IdTipoDictamen",
                        column: x => x.IdTipoDictamen,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Categorias_Productos_UsuarioGeneradorCuil",
                        column: x => x.UsuarioGeneradorCuil,
                        principalTable: "Productos",
                        principalColumn: "Cuil",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categorias_Productos_UsuarioModificacionCuil",
                        column: x => x.UsuarioModificacionCuil,
                        principalTable: "Productos",
                        principalColumn: "Cuil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asunto_UsuarioModificacionCuil",
                table: "Asunto",
                column: "UsuarioModificacionCuil");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_ArchivoLigadoId",
                table: "Categorias",
                column: "ArchivoLigadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_IdAsunto",
                table: "Categorias",
                column: "IdAsunto");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_IdSujetoObligado",
                table: "Categorias",
                column: "IdSujetoObligado");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_IdTipoDictamen",
                table: "Categorias",
                column: "IdTipoDictamen");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UsuarioGeneradorCuil",
                table: "Categorias",
                column: "UsuarioGeneradorCuil");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UsuarioModificacionCuil",
                table: "Categorias",
                column: "UsuarioModificacionCuil");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdTipoSujetoObligado",
                table: "Clientes",
                column: "IdTipoSujetoObligado");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioModificacionCuil",
                table: "Clientes",
                column: "UsuarioModificacionCuil");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UsuarioModificacionCuil",
                table: "Compras",
                column: "UsuarioModificacionCuil");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_UsuarioModificacionCuil",
                table: "Empleados",
                column: "UsuarioModificacionCuil");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "ArchivoPDF");

            migrationBuilder.DropTable(
                name: "Asunto");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
