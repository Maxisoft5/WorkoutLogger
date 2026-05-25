using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;
namespace WorkoutLogg;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("OnboardingProfile", typeof(OnboardingProfilePage));
        Routing.RegisterRoute("OnboardingBody", typeof(OnboardingBodyStatsPage));
        Routing.RegisterRoute("OnboardingGoals", typeof(OnboardingGoalsPage));
        Routing.RegisterRoute("Dashboard", typeof(DashboardPage));
        Routing.RegisterRoute("AddWorkout", typeof(Pages.AddWorkoutPage));
        Routing.RegisterRoute("AddLog", typeof(Pages.AddLogPage));
        Routing.RegisterRoute("Logger", typeof(LoggerPage));
        Routing.RegisterRoute("Profile", typeof(ProfilePage));
        var currentTheme = Application.Current!.RequestedTheme;		
	}

    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
    }

    public static async Task DisplaySnackbarAsync(string message)
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		var snackbarOptions = new SnackbarOptions
		{
			BackgroundColor = Color.FromArgb("#FF3300"),
			TextColor = Colors.White,
			ActionButtonTextColor = Colors.Yellow,
			CornerRadius = new CornerRadius(0),
			Font = Font.SystemFontOfSize(18),
			ActionButtonFont = Font.SystemFontOfSize(14)
		};

		var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

		await snackbar.Show(cancellationTokenSource.Token);
	}

	public static async Task DisplayToastAsync(string message)
	{
		// Toast is currently not working in MCT on Windows
		if (OperatingSystem.IsWindows())
			return;

		var toast = Toast.Make(message, textSize: 18);

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		await toast.Show(cts.Token);
	}

	private void SfSegmentedControl_SelectionChanged(object? sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
    {
		Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
    }
}
