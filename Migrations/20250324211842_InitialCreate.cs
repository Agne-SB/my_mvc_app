using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrunnAvRetur",
                table: "Bestillinger",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "KastetUt",
                table: "Bestillinger",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OutletLagtUt",
                table: "Bestillinger",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Solgt",
                table: "Bestillinger",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrunnAvRetur",
                table: "Bestillinger");

            migrationBuilder.DropColumn(
                name: "KastetUt",
                table: "Bestillinger");

            migrationBuilder.DropColumn(
                name: "OutletLagtUt",
                table: "Bestillinger");

            migrationBuilder.DropColumn(
                name: "Solgt",
                table: "Bestillinger");
        }
    }
}
