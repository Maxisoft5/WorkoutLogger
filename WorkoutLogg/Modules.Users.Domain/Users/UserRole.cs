using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Domain.Users
{
    public class UserRole : IdentityUserRole<string>
    {
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
