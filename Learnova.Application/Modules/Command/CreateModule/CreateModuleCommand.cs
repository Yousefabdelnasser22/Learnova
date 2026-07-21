using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Command.CreateModule
{
   public class CreateModuleCommand:IRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Position { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int CourseId { get; set; }
    }
}
