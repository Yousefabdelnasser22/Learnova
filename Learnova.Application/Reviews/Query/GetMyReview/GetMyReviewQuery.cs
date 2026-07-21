using Learnova.Application.Reviews.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Query.GetMyReview
{
    public class GetMyReviewQuery : IRequest<ReviewDto>
    {
        public int CourseId { get; set; }
    }
}
