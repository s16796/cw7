using cw7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw7.Services
{
    public interface IJWTauthService
    {
        public JWTResponse Login(LoginRequest request);
        public JWTResponse RefreshToken(RefreshRequest refresh);
    }
}
