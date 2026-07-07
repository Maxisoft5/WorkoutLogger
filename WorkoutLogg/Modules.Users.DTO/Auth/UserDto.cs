
using Modules.Users.DTO.Users;
using System.ComponentModel.DataAnnotations;

namespace Modules.Users.DTO.Auth
{
    public class UserDto
    {
        public string? Id { get; set; }

        [MaxLength(256)]
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(128)]
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool AcceptedTerms { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserSex? Identity { get; set; }
        public UserBodyStatsDto? BodyStats { get; set; }
        public WorkOutCountVariant? WorkOutCount { get; set; }
        public UserRegistrationStep? UserRegistrationStep { get; set; }
        public bool? IsPremium { get; set; }
        public List<UserGoalDto>? Goals { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
