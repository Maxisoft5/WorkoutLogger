

using Modules.Common.Domain.Results;

namespace Modules.Users.Domain.Authentication
{
    public class LoginUserResponse : IResult
    {
        public LoginUserResponse(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }

        public LoginUserResponse(List<Error> errors)
        {
            Errors = errors;
        }

        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        public List<Error>? Errors { get; set; } = new();

        public bool IsSuccess => Errors?.Count == 0;

        public bool IsError => Errors?.Count > 0;
    }
}
