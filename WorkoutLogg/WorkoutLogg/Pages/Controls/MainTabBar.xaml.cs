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
        UpdateActiveTab("Dashboard");
    }

    private void UpdateActiveTab(string tab)
    {
        var purple = Color.FromArgb("#7C3AED");
        var gray = Color.FromArgb("#9CA3AF");

        DashboardIcon.TextColor = tab == "Dashboard" ? purple : gray;
        DashboardLabel.TextColor = tab == "Dashboard" ? purple : gray;
        WorkoutsIcon.TextColor = tab == "Workouts" ? purple : gray;
        WorkoutsLabel.TextColor = tab == "Workouts" ? purple : gray;
        LoggerIcon.TextColor = tab == "Logger" ? purple : gray;
        LoggerLabel.TextColor = tab == "Logger" ? purple : gray;
        ProfileIcon.TextColor = tab == "Profile" ? purple : gray;
        ProfileLabel.TextColor = tab == "Profile" ? purple : gray;
    }

    private async void OnDashboardTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Dashboard") return;
        await Shell.Current.GoToAsync("//Dashboard");
    }
    private async void OnWorkoutsTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Workouts") return;
        await Shell.Current.GoToAsync("//Workouts");
    }
    private async void OnLoggerTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Logger") return;
        await Shell.Current.GoToAsync("//Logger");
    }
    private async void OnProfileTapped(object sender, TappedEventArgs e)
    {
        if (ActiveTab == "Profile") return;
        await Shell.Current.GoToAsync("//Profile");
    }
    private async void OnNewWorkoutTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//Logger");
    }
}