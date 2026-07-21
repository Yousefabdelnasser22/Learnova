using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.User
{
    public record CurrentUser(string Id, string Email, IEnumerable<string> Roles)
    {
        public bool IsInRole(string role) => Roles.Contains(role);
    }
}
