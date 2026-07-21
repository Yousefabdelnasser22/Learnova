using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Modules.Command.DeleteModule
{
    public class DeleteModuleCommand(int id ) :IRequest
    {
        public int Id { get; set; } =id;

        public int CourseId { get; set; }


    }
}
