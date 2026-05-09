using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Users.DTO.Users
{
    public class UserGoalDto
    {
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public UserGoalVariant Goal { get; set; }
        public Guid Id { get; set; }
    }
}
