using FluentValidation;
using FluentValidation.Results;
using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Api;
using WorkoutLogg.Validators;

namespace WorkoutLogg.Pages;

public partial class CreateAccount : ContentPage
{
    public IAuthApi AuthApi { get; set; }
    private readonly IValidator<UserDto> _validator;
    public CreateAccount()
    {
        InitializeComponent();
        _validator = new CreateAccountValidator();
        AuthApi = Application.Current!.Handler.MauiContext!.Services
         .GetRequiredService<IAuthApi>();
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        var userDto = new UserDto()
        {
            FullName = FullNameEntry.Text,
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text,
            ConfirmPassword = ConfirmPasswordEntry.Text,
            AcceptedTerms = TermsCheckBox.IsChecked
        };
        ClearErrors();

        ValidationResult result = await _validator.ValidateAsync(userDto);
        if (!result.IsValid)
        {
            ShowErrors(result);
            return;
        }

        Application.Current!.Windows[0].Page = new LoadingPage();

        var created = await AuthApi.CreateAccount(userDto);
        var res = created.Content;

        if (res.IsSuccess && !string.IsNullOrWhiteSpace(res.Value?.Token))
        {
            await LoginService.AddToken(res.Value.Token);
            Application.Current!.Windows[0].Page = new OnboardingProfilePage();
        } 
        else
        {
            await DisplayAlert("Error", string.Join(";", res.Errors.Select(s => s.Description)), "Ok");
        }
    }

    private void ClearErrors()
    {
        SetError(FullNameError, FullNameBorder, null);
        SetError(EmailError, EmailBorder, null);
        SetError(PasswordError, PasswordBorder, null);
        SetError(ConfirmPasswordError, ConfirmPasswordBorder, null);
        TermsCheckBoxError.IsVisible = false;
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
                case nameof(UserDto.FullName):
                    SetError(FullNameError, FullNameBorder, error.ErrorMessage);
                    break;
                case nameof(UserDto.Email):
                    SetError(EmailError, EmailBorder, error.ErrorMessage);
                    break;
                case nameof(UserDto.Password):
                    SetError(PasswordError, PasswordBorder, error.ErrorMessage);
                    break;
                case nameof(UserDto.ConfirmPassword):
                    SetError(ConfirmPasswordError, ConfirmPasswordBorder, error.ErrorMessage);
                    break;
                case nameof(UserDto.AcceptedTerms):
                    TermsCheckBoxError.Text = error.ErrorMessage;
                    TermsCheckBoxError.IsVisible = true;
                    break;
            }
        }
    }

    private void OnTermsCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            CreateAccountGradient1.Color = new Color(124, 58, 237);
            CreateAccountGradient2.Color = new Color(147, 51, 234);
        } 
        else
        {
            CreateAccountGradient1.Color = new Color(228, 221, 235);
            CreateAccountGradient2.Color = new Color(129, 127, 133);
        }
    }

    private async void OnTermsTapped(object sender, EventArgs e)
    {
        // TODO: открыть страницу Terms of Service
        Application.Current!.Windows[0].Page = new TermsOfServicePage();
    }

    private async void OnPrivacyTapped(object sender, EventArgs e)
    {
        // TODO: открыть страницу Privacy Policy
        Application.Current!.Windows[0].Page = new PrivacyPolicy();
    }

    private async void OnAppleSignInClicked(object sender, EventArgs e)
    {
        // TODO: Apple Sign-In
        await DisplayAlert("Apple", "Apple Sign-In — coming soon", "OK");
    }

    private async void OnGoogleSignInClicked(object sender, EventArgs e)
    {
        // TODO: Google Sign-In
        await DisplayAlert("Google", "Google Sign-In — coming soon", "OK");
    }

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage();
    }
}