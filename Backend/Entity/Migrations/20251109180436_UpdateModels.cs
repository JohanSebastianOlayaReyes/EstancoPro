using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstLastName",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "SecondLastName",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "persons");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "persons",
                newName: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "persons",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "FirstLastName",
                table: "persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "persons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "persons",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
