using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.User.Command.UpdateUserDetail
{
    public class UpdateUserDetailCommand :IRequest
    {
        public string? City { get; set; }
        public int? Age { get; set; }
    }
}
