using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Domain.Authentication;
using Modules.Users.DTO.Users;
using Modules.Users.Infrastructure.Api;
using Modules.Users.Infrastructure.Authorization;

namespace WorkoutLogg;

public partial class App : Application
{
    public IAuthApi AuthApi { get; set; }
    public App()
	{
        InitializeComponent();
    }

	protected override Window CreateWindow(IActivationState? activationState)
	{
        var window = new Window(new AppShell());

        _ = InitializeAsync(window);

        return window;
    }

    private async Task InitializeAsync(Window window)
    {
        AuthApi = Handler!.MauiContext!.Services.GetRequiredService<IAuthApi>();

        var isAuthenticated = await LoginService.IsAuthenticated();

        UserRegistrationStep? step = null;
        if (isAuthenticated)
        {
            var token = await LoginService.GetActiveToken();
            var user = await AuthApi.GetCurrentUser($"Bearer {token}");
            if (user.IsSuccessStatusCode)
            {
                step = user.Content?.UserRegistrationStep;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Page targetPage = step switch
                    {
                        UserRegistrationStep.Email => new LoginPage(),
                        UserRegistrationStep.Profile => new OnboardingProfilePage(),
                        UserRegistrationStep.Body => new OnboardingBodyStatsPage(),
                        UserRegistrationStep.Goals => new OnboardingGoalsPage(),
                        UserRegistrationStep.Finished => new DashboardPage(),
                        _ => CreateAppShellWithDashboard()
                    };

                    window.Page = targetPage;
                });
            } 
            else 
            {
                window.Page = new LoginPage();
            }
        } 
        else 
        {
            window.Page = new LoginPage();
        }
    }

    private static AppShell CreateAppShellWithDashboard()
    {
        var shell = new AppShell();
        shell.Loaded += async (_, _) =>
        {
            await shell.GoToAsync("//Dashboard");
        };
        return shell;
    }
}