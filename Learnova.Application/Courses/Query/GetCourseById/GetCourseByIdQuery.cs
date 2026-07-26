using Learnova.Application.Courses.DTO;
using Learnova.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Courses.Query.GetCourseById
{
    public class GetCourseByIdQuery:IRequest<CourseDTO>
    {
        public GetCourseByIdQuery( int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
