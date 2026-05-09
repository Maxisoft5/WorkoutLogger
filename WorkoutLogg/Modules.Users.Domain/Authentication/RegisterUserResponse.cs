using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Domain.Authentication
{
    public sealed record RegisterUserResponse(string Token, string RefreshToken);
  
}
