using WorkoutLogg.Utilities;
using WorkoutLogg.Localization;

namespace WorkoutLogg.Pages.Controls;

public partial class MainTabBar : ContentView
{
    public static readonly BindableProperty ActiveTabProperty =
       BindableProperty.Create(nameof(ActiveTab), typeof(string), typeof(MainTabBar), "Dashboard",
           propertyChanged: (b, _, n) => ((MainTabBar)b).UpdateActiveTab((string)n));

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    public MainTabBar()
    {
        InitializeComponent();
        DashboardLabel.Text = Loc.Get("Tab_Dashboard");
        WorkoutsLabel.Text  = Loc.Get("Tab_Workouts");
        LoggerLabel.Text    = Loc.Get("Tab_Logger");
        AiLabel.Text        = Loc.Get("Tab_AI");
        ProfileLabel.Text   = Loc.Get("Tab_Profile");
        SemanticProperties.SetDescription(DashboardTab, DashboardLabel.Text);
        SemanticProperties.SetDescription(WorkoutsTab, WorkoutsLabel.Text);
        SemanticProperties.SetDescription(LoggerTab, LoggerLabel.Text);
        SemanticProperties.SetDescription(AiTab, AiLabel.Text);
        SemanticProperties.SetDescription(ProfileTab, ProfileLabel.Text);
        UpdateActiveTab("Dashboard");
    }

    private void UpdateActiveTab(string tab)
    {
        var purple = Color.FromArgb("#7C3AED");
        var gray   = Color.FromArgb("#9CA3AF");
        var activeBackground = Color.FromArgb("#EDE9FE");
        DashboardTab.WithThemeColor("BackgroundColor", tab == "Dashboard" ? activeBackground : Colors.Transparent);
        WorkoutsTab.WithThemeColor("BackgroundColor", tab == "Workouts" ? activeBackground : Colors.Transparent);
        LoggerTab.WithThemeColor("BackgroundColor", tab == "Logger" ? activeBackground : Colors.Transparent);
        AiTab.WithThemeColor("BackgroundColor", tab == "AI" ? activeBackground : Colors.Transparent);
        ProfileTab.WithThemeColor("BackgroundColor", tab == "Profile" ? activeBackground : Colors.Transparent);

        DashboardIcon.WithThemeColor("TextColor", tab == "Dashboard" ? purple : gray);
        DashboardLabel.WithThemeColor("TextColor", tab == "Dashboard" ? purple : gray);
        WorkoutsIcon.WithThemeColor("TextColor", tab == "Workouts"  ? purple : gray);
        WorkoutsLabel.WithThemeColor("TextColor", tab == "Workouts"  ? purple : gray);
        LoggerIcon.WithThemeColor("TextColor", tab == "Logger"    ? purple : gray);
        LoggerLabel.WithThemeColor("TextColor", tab == "Logger"    ? purple : gray);
        AiIcon.WithThemeColor("TextColor", tab == "AI"        ? purple : gray);
        AiLabel.WithThemeColor("TextColor", tab == "AI"        ? purple : gray);
        ProfileIcon.WithThemeColor("TextColor", tab == "Profile"   ? purple : gray);
        ProfileLabel.WithThemeColor("TextColor", tab == "Profile"   ? purple : gray);
    }

    private async void OnDashboardTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Dashboard") return;
        await AppShell.NavigateAsync("//Dashboard");
    }
    private async void OnWorkoutsTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Workouts") return;
        await AppShell.NavigateAsync("//Workouts");
    }
    private async void OnLoggerTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Logger") return;
        await AppShell.NavigateAsync("//Logger");
    }
    private async void OnAiTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "AI") return;
        await AppShell.NavigateAsync("//AiCoach");
    }
    private async void OnProfileTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Profile") return;
        await AppShell.NavigateAsync("//Profile");
    }
    private async void OnNewWorkoutTapped(object sender, TappedEventArgs e)
    {
        await AppShell.NavigateAsync("//Logger");
    }
}
