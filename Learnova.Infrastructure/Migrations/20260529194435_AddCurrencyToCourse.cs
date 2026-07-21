using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Courses");
        }
    }
}
