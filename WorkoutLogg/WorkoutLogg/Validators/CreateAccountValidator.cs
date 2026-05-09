using FluentValidation;
using Modules.Users.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutLogg.Validators
{
    public class CreateAccountValidator : AbstractValidator<UserDto>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Введите полное имя")
                .MinimumLength(2).WithMessage("Имя слишком короткое")
                .MaximumLength(100).WithMessage("Имя слишком длинное")
                .Matches(@"^[\p{L}\s'-]+$")
                    .WithMessage("Имя может содержать только буквы, пробелы, дефис и апостроф");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Введите email")
                .EmailAddress().WithMessage("Некорректный email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Введите пароль")
                .MinimumLength(8).WithMessage("Пароль должен быть не короче 8 символов")
                .Matches(@"\d").WithMessage("Пароль должен содержать хотя бы одну цифру");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Подтвердите пароль")
                .Equal(x => x.Password).WithMessage("Пароли не совпадают");

            RuleFor(x => x.AcceptedTerms)
                .Equal(true).WithMessage("Необходимо принять условия");
        }
    }
}
