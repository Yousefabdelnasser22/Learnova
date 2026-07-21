using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Categories.Command.CreateCategory
{
    public class CreateCategoryCommand:IRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
