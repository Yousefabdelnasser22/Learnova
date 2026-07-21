using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesToCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CertificateNo",
                table: "Certificates",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateNo",
                table: "Certificates",
                column: "CertificateNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_StudentId_CourseId",
                table: "Certificates",
                columns: new[] { "StudentId", "CourseId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Certificates_CertificateNo",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_StudentId_CourseId",
                table: "Certificates");

            migrationBuilder.AlterColumn<string>(
                name: "CertificateNo",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);
        }
    }
}

