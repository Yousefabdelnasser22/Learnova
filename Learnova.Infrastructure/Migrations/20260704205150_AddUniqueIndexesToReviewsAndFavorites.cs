using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesToReviewsAndFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FavoriteList_StudentId_CourseId",
                table: "FavoriteList",
                columns: new[] { "StudentId", "CourseId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StudentId_CourseId",
                table: "Reviews",
                columns: new[] { "StudentId", "CourseId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavoriteList_StudentId_CourseId",
                table: "FavoriteList");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_StudentId_CourseId",
                table: "Reviews");
        }
    }
}
