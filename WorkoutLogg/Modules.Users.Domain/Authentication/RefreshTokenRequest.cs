using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Domain.Authentication
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        public string Token { get; set; }
    }
}
