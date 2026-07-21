using Learnova.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnova.Infrastructure.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
