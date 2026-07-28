using Learnova.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260727013000_EnsureCourseLifecycleTimestamps")]
    public partial class EnsureCourseLifecycleTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.Courses', 'SubmittedAt') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Courses] ADD [SubmittedAt] datetime2 NULL;
                END;

                IF COL_LENGTH('dbo.Courses', 'PublishedAt') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Courses] ADD [PublishedAt] datetime2 NULL;
                END;

                IF COL_LENGTH('dbo.Courses', 'ArchivedAt') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Courses] ADD [ArchivedAt] datetime2 NULL;
                END;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.Courses', 'ArchivedAt') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[Courses] DROP COLUMN [ArchivedAt];
                END;

                IF COL_LENGTH('dbo.Courses', 'PublishedAt') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[Courses] DROP COLUMN [PublishedAt];
                END;

                IF COL_LENGTH('dbo.Courses', 'SubmittedAt') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[Courses] DROP COLUMN [SubmittedAt];
                END;
                """);
        }
    }
}
