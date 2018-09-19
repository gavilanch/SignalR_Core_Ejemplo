using Microsoft.EntityFrameworkCore.Migrations;

namespace HolaMundoSignalR.Migrations
{
    public partial class nombre2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nombre2",
                table: "Personas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre2",
                table: "Personas");
        }
    }
}
