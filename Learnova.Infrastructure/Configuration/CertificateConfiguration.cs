using Learnova.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnova.Infrastructure.Configuration
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.Property(x => x.CertificateNo)
                .HasMaxLength(64);

            builder.HasIndex(x => x.CertificateNo)
                .IsUnique()
                .HasDatabaseName("IX_Certificates_CertificateNo");

            builder.HasIndex(x => new { x.StudentId, x.CourseId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_Certificates_StudentId_CourseId");
        }
    }
}
