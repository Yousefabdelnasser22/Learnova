using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addmoduleprogress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // إنشاء جدول ModuleProgress
            migrationBuilder.CreateTable(
                name: "ModuleProgress",
                columns: table => new
                {
                    // تعريف الأعمدة
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),  // تعريف العمود Id كمفتاح رئيسي
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false), // تعديل الطول هنا ليكون 450
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleProgress", x => x.Id);
                    // إضافة المفاتيح الأجنبية
                    table.ForeignKey(
                        name: "FK_ModuleProgress_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade); // Cascade Delete على المستخدمين
                    table.ForeignKey(
                        name: "FK_ModuleProgress_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction); // منع الـ Cascade على الجداول الأخرى
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_ModuleId",
                table: "ModuleProgress",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_StudentId",
                table: "ModuleProgress",
                column: "StudentId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // حذف جدول ModuleProgress عند التراجع عن الترحيل
            migrationBuilder.DropTable(
                name: "ModuleProgress");
        }
    }
    }
