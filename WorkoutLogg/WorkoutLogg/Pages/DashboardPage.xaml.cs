using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;
using WorkoutLogg.Pages.Controls;

namespace WorkoutLogg.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardPageModel _vm;

    public DashboardPage(DashboardPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DateLabel.Text = DateTime.Now.ToString("dddd, dd MMM").ToUpper();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var greeting = Loc.Get("Dashboard_Greeting");
        var currentUser = await CurrentUserStore.GetCurrentUser();
        if (currentUser != null)
        {
            HelloUserText.Text = $"{greeting}, {currentUser.FullName} 👋";
            AvatarLabel.Text = currentUser.FullName?.ToUpper().First().ToString() ?? "?";
        }
        else
        {
            HelloUserText.Text = $"{greeting} 👋";
            AvatarLabel.Text = "?";
        }

        await _vm.LoadAsync();
        ApplyStats();
    }

    private void ApplyStats()
    {
        // Stats cards
        TotalWorkoutsLabel.Text = _vm.TotalWorkoutsLabel;
        WeeklyChangeLabel.Text = _vm.WeeklyChangeLabel;
        StreakLabel.Text = _vm.StreakDaysLabel;
        WeekVolumeLabel.Text = _vm.WeekVolumeLabel;

        // Goals
        WorkoutsGoalPct.Text = _vm.WorkoutsGoalPct;
        VolumeGoalPct.Text = _vm.VolumeGoalPct;
        ConsistencyGoalPct.Text = _vm.ConsistencyGoalPct;
        WorkoutsGoalSub.Text = _vm.WorkoutsGoalSub;
        VolumeGoalSub.Text = _vm.VolumeGoalSub;
        ConsistencyGoalSub.Text = _vm.ConsistencyGoalSub;

        // Last workout
        if (_vm.HasLastWorkout)
        {
            LastWorkoutCard.IsVisible = true;
            NoWorkoutLabel.IsVisible = false;
            LastWorkoutEmoji.Text = _vm.LastEmoji;
            LastWorkoutTitle.Text = _vm.LastTitle;
            LastWorkoutSub.Text = _vm.LastSubLabel;
            LastWorkoutVolume.Text = _vm.LastVolumeLabel;
        }
        else
        {
            LastWorkoutCard.IsVisible = false;
            NoWorkoutLabel.IsVisible = true;
        }

        // Weekly bar chart
        BuildBarChart();
    }

    private void BuildBarChart()
    {
        WeeklyBarsGrid.ColumnDefinitions.Clear();
        WeeklyBarsGrid.Children.Clear();

        var bars = _vm.WeekBars;
        if (bars.Count == 0) return;

        for (int i = 0; i < bars.Count; i++)
            WeeklyBarsGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        for (int i = 0; i < bars.Count; i++)
        {
            var bar = bars[i];

            var barColor = bar.IsToday && bar.HasActivity ? Color.FromArgb("#7C3AED")
                : bar.HasActivity ? Color.FromArgb("#C4B5FD")
                : Color.FromArgb("#F3F4F6");

            var labelColor = bar.IsToday
                ? Color.FromArgb("#7C3AED")
                : Color.FromArgb("#9CA3AF");

            var barBorder = new Border
            {
                BackgroundColor = barColor,
                HeightRequest = bar.BarHeight,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                StrokeThickness = 0,
            };
            Grid.SetRow(barBorder, 1);

            var dayLabel = new Label
            {
                Text = bar.DayLabel,
                FontSize = 10,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = labelColor,
                FontAttributes = bar.IsToday ? FontAttributes.Bold : FontAttributes.None,
                Margin = new Thickness(0, 4, 0, 0),
            };
            Grid.SetRow(dayLabel, 2);

            var col = new Grid
            {
                RowDefinitions = new RowDefinitionCollection(
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto)),
            };
            col.Children.Add(barBorder);
            col.Children.Add(dayLabel);

            Grid.SetColumn(col, i);
            WeeklyBarsGrid.Children.Add(col);
        }
    }

    private async void OnSeeAllWorkoutsTapped(object sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//Logger");
}
