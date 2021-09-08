using Microsoft.EntityFrameworkCore.Migrations;

namespace Dictamenes.Migrations
{
    public partial class SegundaVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "Empleados",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "Compras",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "Asunto",
                newName: "Descripcion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Empleados",
                newName: "descripcion");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Compras",
                newName: "descripcion");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Asunto",
                newName: "descripcion");
        }
    }
}
