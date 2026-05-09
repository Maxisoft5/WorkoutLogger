using FluentValidation;
using FluentValidation.Results;
using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Api;
using WorkoutLogg.Validators;

namespace WorkoutLogg.Pages;

public partial class LoginPage : ContentPage
{
    public IAuthApi AuthApi { get; set; }
    private IValidator<UserDto> _loginToAccountValidator;
    public LoginPage()
	{
		InitializeComponent();
        AuthApi = Application.Current!.Handler.MauiContext!.Services
          .GetRequiredService<IAuthApi>();
        _loginToAccountValidator = new LoginToAccountValidator();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        var userDto = new UserDto()
        {
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };
        ClearErrors();

        var result = _loginToAccountValidator.Validate(userDto);
        if (!result.IsValid)
        {
            ShowErrors(result);
            return;
        }

        Application.Current!.Windows[0].Page = new LoadingPage();

        var loginRes = await AuthApi.Login(userDto);
        var res = loginRes.Content;
        if (res.IsSuccess && !string.IsNullOrWhiteSpace(res.Value?.Token))
        {
            var currentUser = await AuthApi.GetCurrentUser($"Bearer {res.Value.Token}");
            await LoginService.AddToken(res.Value.Token);
            if (currentUser.IsSuccessful && currentUser.Content.UserRegistrationStep 
                == Modules.Users.DTO.Users.UserRegistrationStep.Profile)
            {
                Application.Current!.Windows[0].Page = new OnboardingProfilePage();
            }
            if (currentUser.IsSuccessful && currentUser.Content.UserRegistrationStep 
                == Modules.Users.DTO.Users.UserRegistrationStep.Body)
            {
                Application.Current!.Windows[0].Page = new OnboardingBodyStatsPage();
            }
            if (currentUser.IsSuccessful && currentUser.Content.UserRegistrationStep
              == Modules.Users.DTO.Users.UserRegistrationStep.Goals)
            {
                Application.Current!.Windows[0].Page = new OnboardingGoalsPage();
            }
            if (currentUser.IsSuccessful && currentUser.Content.UserRegistrationStep
             == Modules.Users.DTO.Users.UserRegistrationStep.Finished)
            {
                Application.Current!.Windows[0].Page = new AppShell();
                Application.Current!.Windows[0].Page.Loaded += async (_, _) =>
                {
                    await Shell.Current.GoToAsync("//Dashboard");
                };
            }
        }
    }

    private void ClearErrors()
    {
        SetError(EmailError, EmailBorder, null);
        SetError(PasswordError, PasswordBorder, null);
    }

    private static void SetError(Label errorLabel, Border border, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            errorLabel.IsVisible = false;
            border.Stroke = Color.FromArgb("#E5E7EB");
        }
        else
        {
            errorLabel.Text = message;
            errorLabel.IsVisible = true;
            border.Stroke = Color.FromArgb("#EF4444");
        }
    }

    private void ShowErrors(ValidationResult result)
    {
        foreach (var error in result.Errors)
        {
            switch (error.PropertyName)
            {
                case nameof(UserDto.Email):
                    SetError(EmailError, EmailBorder, error.ErrorMessage);
                    break;
                case nameof(UserDto.Password):
                    SetError(PasswordError, PasswordBorder, error.ErrorMessage);
                    break;
            }
        }
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        // TODO: навигация на ForgotPasswordPage
        Application.Current!.Windows[0].Page = new ForgotPassword();
    }

    private async void OnAppleSignInClicked(object sender, EventArgs e)
    {
        // TODO: Apple Sign-In через AppleSignInAuthenticator
        await DisplayAlert("Apple", "Apple Sign-In — coming soon", "OK");
    }

    private async void OnGoogleSignInClicked(object sender, EventArgs e)
    {
        // TODO: Google Sign-In через WebAuthenticator / Google Auth
        await DisplayAlert("Google", "Google Sign-In — coming soon", "OK");
    }

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        // TODO: навигация на RegisterPage
        Application.Current!.Windows[0].Page = new CreateAccount();
    }
}