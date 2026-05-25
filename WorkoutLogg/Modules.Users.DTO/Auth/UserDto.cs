
using Modules.Users.DTO.Users;

namespace Modules.Users.DTO.Auth
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
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
    }
}
