using WorkoutLogg.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Domain.Authentication;
using Modules.Users.DTO.Users;
using Modules.Users.Infrastructure.Api;
using Modules.Users.Infrastructure.Authorization;
using System.Diagnostics;
using System.Net;
using WorkoutLogg.Localization;

namespace WorkoutLogg;

public partial class App : Application
{
    public IAuthApi AuthApi { get; set; }
    public App()
	{
        InitializeComponent();
        Services.ThemeService.Initialize();
    }

	protected override Window CreateWindow(IActivationState? activationState)
	{
        var window = new Window();
        ShowStartupLoading(window);
        return window;
    }

    private void ShowStartupLoading(Window window)
    {
        var loadingPage = new LoadingPage();
        EventHandler? loaded = null;
        loaded = async (_, _) =>
        {
            loadingPage.Loaded -= loaded;
            await InitializeAsync(window);
        };
        // Подписываемся до установки Page: Loaded не должен быть пропущен.
        loadingPage.Loaded += loaded;
        window.Page = loadingPage;
    }

    private async Task InitializeAsync(Window window)
    {
        var loadingPage = window.Page;
        try
        {
            AuthApi = Handler!.MauiContext!.Services.GetRequiredService<IAuthApi>();
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                window.Page = new LoginPage();
                return;
            }

            // HttpClient авторизации ограничивает запрос 30 секундами.
            using var user = await AuthApi.GetCurrentUser($"Bearer {token}");
            // AuthHeaderHandler мог уже направить пользователя на вход.
            if (window.Page != loadingPage)
                return;

            if (user.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                window.Page = new LoginPage();
                return;
            }

            if (!user.IsSuccessful || user.Content is null)
            {
                window.Page = CreateStartupErrorPage(window, Loc.Get("Common_TryAgain"));
                return;
            }

            await CurrentUserStore.SetCurrentUser(user.Content);
            window.Page = user.Content.UserRegistrationStep switch
            {
                UserRegistrationStep.Email => new LoginPage(),
                UserRegistrationStep.Profile => new OnboardingProfilePage(),
                UserRegistrationStep.Body => new OnboardingBodyStatsPage(),
                UserRegistrationStep.Goals => new OnboardingGoalsPage(),
                UserRegistrationStep.Finished => CreateAppShellWithDashboard(window),
                _ => CreateStartupErrorPage(window, Loc.Get("Common_TryAgain"))
            };
        }
        catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
        {
            Debug.WriteLine($"[Startup] Request timed out: {ex.GetType().Name}");
            if (window.Page == loadingPage)
                window.Page = CreateStartupErrorPage(window, Loc.Get("Common_RequestTimeout"));
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"[Startup] Connection failed: {ex}");
            if (window.Page == loadingPage)
                window.Page = CreateStartupErrorPage(window, Loc.Get("Common_ConnectionError"));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Startup] Initialization failed: {ex}");
            if (window.Page == loadingPage)
                window.Page = CreateStartupErrorPage(window, Loc.Get("Common_TryAgain"));
        }
    }

    private Page CreateStartupErrorPage(Window window, string message)
    {
        var retryButton = new Button
        {
            Text = Loc.Get("Startup_Retry"),
            CornerRadius = 14,
            MinimumHeightRequest = 48
        }.WithThemeColor("BackgroundColor", Color.FromArgb("#7C3AED")).WithThemeColor("TextColor", Colors.White);
        retryButton.Clicked += (_, _) =>
        {
            retryButton.IsEnabled = false;
            ShowStartupLoading(window);
        };

        var loginButton = new Button
        {
            Text = Loc.Get("Login_SignIn"),
            MinimumHeightRequest = 48
        }.WithThemeColor("BackgroundColor", Colors.Transparent).WithThemeColor("TextColor", Color.FromArgb("#7C3AED"));
        loginButton.Clicked += (_, _) => window.Page = new LoginPage();

        return new ContentPage
        {
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = new Thickness(28, 60, 28, 40),
                    Spacing = 20,
                    Children =
                    {
                        new Label
                        {
                            Text = Loc.Get("Startup_ErrorTitle"),FontSize = 24,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.WordWrap
                        }.WithThemeColor("TextColor", Color.FromArgb("#111827")),
                        new Label
                        {
                            Text = message,FontSize = 16,
                            LineBreakMode = LineBreakMode.WordWrap
                        }.WithThemeColor("TextColor", Color.FromArgb("#6B7280")),
                        retryButton,
                        loginButton
                    }
                }
            }
        }.WithThemeColor("BackgroundColor", Color.FromArgb("#EEEEF6"));
    }

    private AppShell CreateAppShellWithDashboard(Window window)
    {
        var shell = new AppShell();
        EventHandler? loaded = null;
        loaded = async (_, _) =>
        {
            shell.Loaded -= loaded;
            try
            {
                if (window.Page == shell)
                    await shell.GoToAsync("//Dashboard", animate: false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Startup] Dashboard navigation failed: {ex}");
                if (window.Page == shell)
                    window.Page = CreateStartupErrorPage(window, Loc.Get("Common_TryAgain"));
            }
        };
        shell.Loaded += loaded;
        return shell;
    }
}
