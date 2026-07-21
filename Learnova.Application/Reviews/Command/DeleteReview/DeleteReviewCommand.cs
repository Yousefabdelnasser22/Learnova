using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Command.DeleteReview
{
    public class DeleteReviewCommand : IRequest<bool>
    {
        public int ReviewId { get; set; }
    }
}
