using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Command.ReorderModule
{
    public class ReorderModuleCommand : IRequest
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int CourseId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public int ModuleId { get; set; }
        public int NewPosition { get; set; }
    }
}
