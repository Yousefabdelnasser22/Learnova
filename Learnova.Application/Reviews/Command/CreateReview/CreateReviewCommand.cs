using Learnova.Application.Reviews.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Reviews.Command.CreateReview
{
    public class CreateReviewCommand : IRequest<ReviewDto>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int CourseId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
