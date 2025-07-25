using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heimlich.Application.DTOs
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
