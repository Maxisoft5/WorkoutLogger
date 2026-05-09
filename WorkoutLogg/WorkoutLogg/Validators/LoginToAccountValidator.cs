using FluentValidation;
using Modules.Users.DTO.Auth;


namespace WorkoutLogg.Validators
{
    public class LoginToAccountValidator : AbstractValidator<UserDto>
    {
        public LoginToAccountValidator()
        {
            RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Введите email")
                    .EmailAddress().WithMessage("Некорректный email");
            RuleFor(x => x.Password)
              .NotEmpty().WithMessage("Введите пароль");
        }
    }
}
