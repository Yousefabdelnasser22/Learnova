using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.User.Command.AssignUserRole
{
    public class UnassignUserRoleCommand : IRequest
    {
        public string UserEmail { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;
    }
}
