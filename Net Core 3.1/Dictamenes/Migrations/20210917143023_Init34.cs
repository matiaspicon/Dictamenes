using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictamenes.Migrations
{
    public partial class Init34 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivoPDF",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(nullable: true),
                    TipoArchivo = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    FechaCarga = table.Column<DateTime>(nullable: false),
                    Contenido = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivoPDF", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Busqueda",
                columns: table => new
                {
                    NroGDE = table.Column<string>(nullable: true),
                    NroExp = table.Column<string>(nullable: true),
                    FechaCargaInicio = table.Column<DateTime>(nullable: true),
                    FechaCargaFinal = table.Column<DateTime>(nullable: true),
                    Contenido = table.Column<string>(nullable: true),
                    Detalle = table.Column<string>(nullable: true),
                    IdAsunto = table.Column<int>(nullable: true),
                    IdTipoSujetoObligado = table.Column<int>(nullable: true),
                    IdTipoDictamen = table.Column<int>(nullable: true),
                    EsDenunciante = table.Column<bool>(nullable: false),
                    IdSujetoObligado = table.Column<int>(nullable: true),
                    CuilCuit = table.Column<int>(nullable: true),
                    Nombre = table.Column<string>(nullable: true),
                    Apellido = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cuil = table.Column<long>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Asunto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descripcion = table.Column<string>(maxLength: 50, nullable: true),
                    EstaHabilitado = table.Column<bool>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asunto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asunto_Usuario_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TipoDictamen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descripcion = table.Column<string>(maxLength: 50, nullable: true),
                    EstaHabilitado = table.Column<bool>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDictamen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipoDictamen_Usuario_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TipoSujetoObligado",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descripcion = table.Column<string>(maxLength: 50, nullable: true),
                    EstaHabilitado = table.Column<bool>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoSujetoObligado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipoSujetoObligado_Usuario_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SujetoObligado",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CuilCuit = table.Column<long>(nullable: false),
                    Nombre = table.Column<string>(maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(maxLength: 50, nullable: true),
                    RazonSocial = table.Column<string>(maxLength: 80, nullable: true),
                    IdTipoSujetoObligado = table.Column<int>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SujetoObligado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SujetoObligado_TipoSujetoObligado_IdTipoSujetoObligado",
                        column: x => x.IdTipoSujetoObligado,
                        principalTable: "TipoSujetoObligado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SujetoObligado_Usuario_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dictamenes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NroGDE = table.Column<string>(maxLength: 30, nullable: false),
                    NroExpediente = table.Column<string>(maxLength: 30, nullable: false),
                    FechaCarga = table.Column<DateTime>(nullable: false),
                    Detalle = table.Column<string>(nullable: true),
                    EsPublico = table.Column<bool>(nullable: false),
                    IdArchivoPDF = table.Column<int>(nullable: true),
                    IdSujetoObligado = table.Column<int>(nullable: false),
                    IdAsunto = table.Column<int>(nullable: false),
                    IdTipoDictamen = table.Column<int>(nullable: false),
                    Borrado = table.Column<bool>(nullable: false),
                    EstaActivo = table.Column<bool>(nullable: false),
                    FechaModificacion = table.Column<DateTime>(nullable: false),
                    IdUsuarioModificacion = table.Column<int>(nullable: false),
                    UsuarioModificacionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictamenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dictamenes_ArchivoPDF_IdArchivoPDF",
                        column: x => x.IdArchivoPDF,
                        principalTable: "ArchivoPDF",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dictamenes_Asunto_IdAsunto",
                        column: x => x.IdAsunto,
                        principalTable: "Asunto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dictamenes_SujetoObligado_IdSujetoObligado",
                        column: x => x.IdSujetoObligado,
                        principalTable: "SujetoObligado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dictamenes_TipoDictamen_IdTipoDictamen",
                        column: x => x.IdTipoDictamen,
                        principalTable: "TipoDictamen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dictamenes_Usuario_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asunto_UsuarioModificacionId",
                table: "Asunto",
                column: "UsuarioModificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamenes_IdArchivoPDF",
                table: "Dictamenes",
                column: "IdArchivoPDF");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamenes_IdAsunto",
                table: "Dictamenes",
                column: "IdAsunto");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamenes_IdSujetoObligado",
                table: "Dictamenes",
                column: "IdSujetoObligado");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamenes_IdTipoDictamen",
                table: "Dictamenes",
                column: "IdTipoDictamen");

            migrationBuilder.CreateIndex(
                name: "IX_Dictamenes_UsuarioModificacionId",
                table: "Dictamenes",
                column: "UsuarioModificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_SujetoObligado_IdTipoSujetoObligado",
                table: "SujetoObligado",
                column: "IdTipoSujetoObligado");

            migrationBuilder.CreateIndex(
                name: "IX_SujetoObligado_UsuarioModificacionId",
                table: "SujetoObligado",
                column: "UsuarioModificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoDictamen_UsuarioModificacionId",
                table: "TipoDictamen",
                column: "UsuarioModificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoSujetoObligado_UsuarioModificacionId",
                table: "TipoSujetoObligado",
                column: "UsuarioModificacionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Busqueda");

            migrationBuilder.DropTable(
                name: "Dictamenes");

            migrationBuilder.DropTable(
                name: "ArchivoPDF");

            migrationBuilder.DropTable(
                name: "Asunto");

            migrationBuilder.DropTable(
                name: "SujetoObligado");

            migrationBuilder.DropTable(
                name: "TipoDictamen");

            migrationBuilder.DropTable(
                name: "TipoSujetoObligado");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
