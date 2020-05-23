using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw7.Models
{
    public class JWTResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
