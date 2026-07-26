using Learnova.Domain.Entities;
using Learnova.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Payments.Specifications
{
    public class ProcessedWebhookEventByEventIdSpecification
    : BaseSpecification<ProcessedWebhookEvent>
    {
        public ProcessedWebhookEventByEventIdSpecification(string provider, string eventId)
            : base(x => x.Provider == provider && x.EventId == eventId)
        {
        }
    }
}
