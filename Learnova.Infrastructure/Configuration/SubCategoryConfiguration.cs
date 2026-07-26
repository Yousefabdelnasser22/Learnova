using Learnova.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnova.Infrastructure.Configuration
{
    public class SubCategoryConfiguration : IEntityTypeConfiguration<SubCategory>
    {
        public void Configure(EntityTypeBuilder<SubCategory> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(100);

            builder.HasIndex(x => new { x.CategoryId, x.Name })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
