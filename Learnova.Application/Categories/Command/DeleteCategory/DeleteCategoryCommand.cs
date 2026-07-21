using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Categories.Command.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest
    {
        public int CategoryId { get; set; }
    }
}
