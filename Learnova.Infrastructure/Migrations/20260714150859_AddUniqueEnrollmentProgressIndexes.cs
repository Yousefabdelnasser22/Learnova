using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueEnrollmentProgressIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ;WITH RankedEnrollments AS
                (
                    SELECT
                        [Id],
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY [StudentId], [CourseId]
                            ORDER BY
                                CASE
                                    WHEN [IsCompleted] = 1 OR [Status] = 3
                                    THEN 1
                                    ELSE 0
                                END DESC,
                                [ProgressPercentage] DESC,
                                [EnrolledAt] ASC,
                                [Id] ASC
                        ) AS [RowNumber]
                    FROM [dbo].[Enrollments]
                    WHERE [IsDeleted] = 0
                )
                UPDATE [dbo].[Enrollments]
                SET [IsDeleted] = 1
                WHERE [Id] IN
                (
                    SELECT [Id]
                    FROM RankedEnrollments
                    WHERE [RowNumber] > 1
                );

                ;WITH RankedLessonProgress AS
                (
                    SELECT
                        [Id],
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY [StudentId], [LessonId]
                            ORDER BY
                                [IsCompleted] DESC,
                                [CompletedAt] DESC,
                                [Id] ASC
                        ) AS [RowNumber]
                    FROM [dbo].[LessonProgress]
                    WHERE [IsDeleted] = 0
                )
                UPDATE [dbo].[LessonProgress]
                SET [IsDeleted] = 1
                WHERE [Id] IN
                (
                    SELECT [Id]
                    FROM RankedLessonProgress
                    WHERE [RowNumber] > 1
                );

                ;WITH RankedModuleProgress AS
                (
                    SELECT
                        [Id],
                        ROW_NUMBER() OVER
                        (
                            PARTITION BY [StudentId], [ModuleId]
                            ORDER BY
                                [IsCompleted] DESC,
                                [CompletedAt] DESC,
                                [Id] ASC
                        ) AS [RowNumber]
                    FROM [dbo].[ModuleProgress]
                    WHERE [IsDeleted] = 0
                )
                UPDATE [dbo].[ModuleProgress]
                SET [IsDeleted] = 1
                WHERE [Id] IN
                (
                    SELECT [Id]
                    FROM RankedModuleProgress
                    WHERE [RowNumber] > 1
                );
                """);

            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS [IX_ModuleProgress_StudentId]
                ON [dbo].[ModuleProgress];

                DROP INDEX IF EXISTS [IX_ModuleProgress_StudentId_ModuleId]
                ON [dbo].[ModuleProgress];

                DROP INDEX IF EXISTS [IX_LessonProgress_StudentId]
                ON [dbo].[LessonProgress];

                DROP INDEX IF EXISTS [IX_LessonProgress_StudentId_LessonId]
                ON [dbo].[LessonProgress];

                DROP INDEX IF EXISTS [IX_Enrollments_StudentId]
                ON [dbo].[Enrollments];

                DROP INDEX IF EXISTS [IX_Enrollments_StudentId_CourseId]
                ON [dbo].[Enrollments];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_StudentId_ModuleId",
                table: "ModuleProgress",
                columns: new[]
                {
                    "StudentId",
                    "ModuleId"
                },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_StudentId_LessonId",
                table: "LessonProgress",
                columns: new[]
                {
                    "StudentId",
                    "LessonId"
                },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId",
                table: "Enrollments",
                columns: new[]
                {
                    "StudentId",
                    "CourseId"
                },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS [IX_ModuleProgress_StudentId_ModuleId]
                ON [dbo].[ModuleProgress];

                DROP INDEX IF EXISTS [IX_LessonProgress_StudentId_LessonId]
                ON [dbo].[LessonProgress];

                DROP INDEX IF EXISTS [IX_Enrollments_StudentId_CourseId]
                ON [dbo].[Enrollments];

                DROP INDEX IF EXISTS [IX_ModuleProgress_StudentId]
                ON [dbo].[ModuleProgress];

                DROP INDEX IF EXISTS [IX_LessonProgress_StudentId]
                ON [dbo].[LessonProgress];

                DROP INDEX IF EXISTS [IX_Enrollments_StudentId]
                ON [dbo].[Enrollments];
                """);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_StudentId",
                table: "ModuleProgress",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_StudentId",
                table: "LessonProgress",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");
        }
    }
}
