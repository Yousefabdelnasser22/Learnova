using Learnova.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnova.Infrastructure.Configuration
{
    public class ProcessedWebhookEventConfiguration : IEntityTypeConfiguration<ProcessedWebhookEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedWebhookEvent> builder)
        {
            builder.HasIndex(x => new { x.Provider, x.EventId })
                   .IsUnique()
                   .HasDatabaseName("IX_ProcessedWebhookEvents_Provider_EventId");
        }
    }
}
