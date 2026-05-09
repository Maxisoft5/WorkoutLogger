using FluentValidation;
using Modules.Users.DTO.Auth;
using WorkoutLogg.Validators;

namespace WorkoutLogg.Utilities
{
    public static class CollectionServicesExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidator<UserDto>, CreateAccountValidator>();
            return services;
        }
    }
}
