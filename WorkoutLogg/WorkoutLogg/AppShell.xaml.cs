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
        Routing.RegisterRoute("EditBodyStats", typeof(Pages.EditBodyStatsPage));
        Routing.RegisterRoute("Standards", typeof(Pages.StandardsPage));
        Routing.RegisterRoute("Premium", typeof(Pages.PremiumPage));
        Routing.RegisterRoute("AiCoach", typeof(Pages.AiCoachPage));
        Routing.RegisterRoute("PremiumCompare", typeof(Pages.PremiumComparePage));
        Routing.RegisterRoute("Payment", typeof(Pages.PaymentPage));
        Routing.RegisterRoute("Logger", typeof(LoggerPage));
        Routing.RegisterRoute("Profile", typeof(ProfilePage));
        Routing.RegisterRoute("Trainers", typeof(Pages.TrainersPage));
        Routing.RegisterRoute("TrainerDetail", typeof(Pages.TrainerDetailPage));
        Routing.RegisterRoute("Wallet", typeof(Pages.WalletPage));
        Routing.RegisterRoute("ChatList", typeof(Pages.ChatListPage));
        Routing.RegisterRoute("ChatThread", typeof(Pages.ChatThreadPage));
        Routing.RegisterRoute("MyBookings", typeof(Pages.MyBookingsPage));
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
