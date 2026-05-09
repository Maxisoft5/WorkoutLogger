using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.Infrastructure.Authorization
{
    public class NoDuplicateUserNameValidator<TUser>(IdentityErrorDescriber? errors = null)
    : IUserValidator<TUser> where TUser : class
    {
        private readonly IdentityErrorDescriber _describer = errors ?? new IdentityErrorDescriber();

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var errors = new List<IdentityError>();

            if (manager.Options.User.RequireUniqueEmail)
            {
                var email = await manager.GetEmailAsync(user);
                if (string.IsNullOrWhiteSpace(email))
                {
                    errors.Add(_describer.InvalidEmail(email));
                }
                else
                {
                    var owner = await manager.FindByEmailAsync(email);
                    if (owner is not null &&
                        !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
                    {
                        errors.Add(_describer.DuplicateEmail(email));
                    }
                }
            }

            return errors.Count == 0
                ? IdentityResult.Success
                : IdentityResult.Failed(errors.ToArray());
        }
    }
}
