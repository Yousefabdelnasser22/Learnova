using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Common
{
    public static class CertificateNumberGenerator
    {
        public static string Generate()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"EDU-{date}-{random}";
        }
    }
}
