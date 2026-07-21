using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPassProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPass",
                table: "QuizAttempts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPass",
                table: "QuizAttempts");
        }
    }
}
