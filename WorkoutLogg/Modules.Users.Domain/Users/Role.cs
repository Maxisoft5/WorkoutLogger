using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Domain.Users
{
    public class Role : IdentityRole
    {
        public ICollection<UserRole> UserRoles { get; set; } = null!;
        public ICollection<RoleClaim> RoleClaims { get; set; } = null!;
    }
}
