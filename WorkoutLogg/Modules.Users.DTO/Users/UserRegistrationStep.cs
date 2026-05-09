using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.DTO.Users
{
    public enum UserRegistrationStep
    {
        Email = 0,
        Profile,
        Body,
        Goals,
        Finished
    }
}
