using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class ProcessedWebhookEvent : BaseEntity
    {
        public string Provider { get; set; } = null!;

        public string EventId { get; set; } = null!;

        public string EventType { get; set; } = null!;

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
