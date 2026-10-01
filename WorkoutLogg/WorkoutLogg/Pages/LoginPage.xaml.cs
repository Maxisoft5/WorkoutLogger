using WorkoutLogg.Utilities;
using FluentValidation;
using FluentValidation.Results;
using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Api;
using WorkoutLogg.Localization;
using WorkoutLogg.Validators;

namespace WorkoutLogg.Pages;

public partial class LoginPage : ContentPage
{
    public IAuthApi AuthApi { get; set; }
    private IValidator<UserDto> _loginToAccountValidator;
    private bool _isSubmitting;
    public LoginPage()
	{
		InitializeComponent();
        AuthApi = Application.Current!.Handler.MauiContext!.Services
          .GetRequiredService<IAuthApi>();
        _loginToAccountValidator = new LoginToAccountValidator();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        if (_isSubmitting)
            return;

        _isSubmitting = true;
        var window = Application.Current!.Windows[0];
        var loadingPage = new LoadingPage();

        try
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

            window.Page = loadingPage;

            using var loginRes = await AuthApi.Login(userDto);
            var res = loginRes.Content;
            if (loginRes.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(res?.Token))
            {
                await LoginService.AddToken(res.Token);
                using var currentUser = await AuthApi.GetCurrentUser($"Bearer {res.Token}");
                if (!currentUser.IsSuccessful || currentUser.Content == null)
                {
                    await ShowRequestErrorAsync(ApiProblem.GetDetail(currentUser, Loc.Get("Common_TryAgain")));
                    return;
                }

                await CurrentUserStore.SetCurrentUser(currentUser.Content);
                Page? nextPage = currentUser.Content.UserRegistrationStep switch
                {
                    Modules.Users.DTO.Users.UserRegistrationStep.Profile => new OnboardingProfilePage(),
                    Modules.Users.DTO.Users.UserRegistrationStep.Body => new OnboardingBodyStatsPage(),
                    Modules.Users.DTO.Users.UserRegistrationStep.Goals => new OnboardingGoalsPage(),
                    Modules.Users.DTO.Users.UserRegistrationStep.Finished => new AppShell(),
                    _ => null
                };

                if (nextPage == null)
                {
                    await ShowRequestErrorAsync(Loc.Get("Common_TryAgain"));
                    return;
                }

                if (nextPage is AppShell shell)
                {
                    shell.Loaded += async (_, _) =>
                    {
                        await shell.GoToAsync("//Dashboard");
                    };
                }
                window.Page = nextPage;
            }
            else
            {
                await ShowRequestErrorAsync(ApiProblem.GetDetail(loginRes, Loc.Get("Login_InvalidCredentials")));
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
        {
            await ShowRequestErrorAsync(Loc.Get("Common_RequestTimeout"));
        }
        catch (HttpRequestException)
        {
            await ShowRequestErrorAsync(Loc.Get("Common_ConnectionError"));
        }
        catch (Exception)
        {
            await ShowRequestErrorAsync(Loc.Get("Common_TryAgain"));
        }
        finally
        {
            if (window.Page == loadingPage)
                window.Page = this;
            _isSubmitting = false;
        }

        async Task ShowRequestErrorAsync(string message)
        {
            window.Page = this;
            await DisplayAlertAsync(Loc.Get("Common_Error"), message, Loc.Get("Common_OK"));
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
            border.WithThemeColor("Stroke", Color.FromArgb("#E5E7EB"));
        }
        else
        {
            errorLabel.Text = message;
            errorLabel.IsVisible = true;
            border.WithThemeColor("Stroke", Color.FromArgb("#EF4444"));
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
        Application.Current!.Windows[0].Page = new ForgotPassword();
    }

    private void OnTogglePasswordVisibility(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        PasswordEyeImage.Source = new FontImageSource
        {
            Glyph = PasswordEntry.IsPassword ? FluentUI.eye_20_regular : FluentUI.eye_off_20_regular,
            FontFamily = FluentUI.FontFamily,
            Size = 20
        }.WithThemeColor("Color", Color.FromArgb("#9CA3AF"));
    }

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new CreateAccount();
    }
}
