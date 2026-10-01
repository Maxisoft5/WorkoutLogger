using WorkoutLogg.Utilities;
using FluentValidation;
using FluentValidation.Results;
using Modules.Users.DTO.Auth;
using Modules.Users.Infrastructure.Api;
using WorkoutLogg.Localization;
using WorkoutLogg.Validators;

namespace WorkoutLogg.Pages;

public partial class CreateAccount : ContentPage
{
    public IAuthApi AuthApi { get; set; }
    private readonly IValidator<UserDto> _validator;
    private bool _isSubmitting;
    public CreateAccount()
    {
        InitializeComponent();
        _validator = new CreateAccountValidator();
        AuthApi = Application.Current!.Handler.MauiContext!.Services
         .GetRequiredService<IAuthApi>();
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
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

            window.Page = loadingPage;

            using var created = await AuthApi.CreateAccount(userDto);
            var res = created.Content;

            if (created.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(res?.Token))
            {
                await LoginService.AddToken(res.Token);
                window.Page = new OnboardingProfilePage();
            }
            else
            {
                await ShowRequestErrorAsync(ApiProblem.GetDetail(created, Loc.Get("Common_TryAgain")));
            }
        }
        catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
        {
            await ShowRequestErrorAsync(Loc.Get("Common_RequestTimeout"));
        }
        catch (HttpRequestException ex)
        {
            await ShowRequestErrorAsync(Loc.Get("Common_ConnectionError"));
        }
        catch (Exception ex)
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
            CreateAccountGradient1.WithThemeColor("Color", new Color(124, 58, 237));
            CreateAccountGradient2.WithThemeColor("Color", new Color(147, 51, 234));
        } 
        else
        {
            CreateAccountGradient1.WithThemeColor("Color", new Color(228, 221, 235));
            CreateAccountGradient2.WithThemeColor("Color", new Color(129, 127, 133));
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

    private void OnToggleConfirmPasswordVisibility(object sender, EventArgs e)
    {
        ConfirmPasswordEntry.IsPassword = !ConfirmPasswordEntry.IsPassword;
        ConfirmPasswordEyeImage.Source = new FontImageSource
        {
            Glyph = ConfirmPasswordEntry.IsPassword ? FluentUI.eye_20_regular : FluentUI.eye_off_20_regular,
            FontFamily = FluentUI.FontFamily,
            Size = 20
        }.WithThemeColor("Color", Color.FromArgb("#9CA3AF"));
    }

    private async void OnSignInTapped(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage();
    }
}
