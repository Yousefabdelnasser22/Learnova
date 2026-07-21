using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Categories.Command.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
