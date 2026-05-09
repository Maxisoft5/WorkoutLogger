using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Domain.Users
{
    public class UserClaim : IdentityUserClaim<string>
    {
        public User User { get; set; } = null!;
    }
}
