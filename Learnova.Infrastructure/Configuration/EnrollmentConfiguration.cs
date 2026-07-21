using Learnova.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnova.Infrastructure.Configuration
{
    public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasIndex(enrollment => new
                {
                    enrollment.StudentId,
                    enrollment.CourseId
                })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_Enrollments_StudentId_CourseId");
        }
    }
}
