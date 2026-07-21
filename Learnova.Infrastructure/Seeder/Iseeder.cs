using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Seeder
{
    public interface Iseeder
    {
          Task seed();

        IEnumerable<IdentityRole> GetRole();
    }
}
