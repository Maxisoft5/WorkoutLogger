using Moduels.Workouts.DTO.Enums;
using WorkoutLogg.PageModels;

namespace WorkoutLogg.Pages;

public partial class WorkoutsPage : ContentPage
{
    private readonly WorkoutsPageModel _vm;
    private string _activeFilter = "All";

    public WorkoutsPage(WorkoutsPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
        SessionCountLabel.Text = _vm.SessionCount;
        Calendar.MarkedDates = _vm.MarkedDates;
    }

    private async void OnAddWorkoutTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("AddWorkout");
    }

    private async void OnWorkoutOptionsTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Guid workoutId) return;

        var action = await DisplayActionSheet(null, "Cancel", null, "Edit", "Delete");

        if (action == "Edit")
            await Shell.Current.GoToAsync($"AddWorkout?workoutId={workoutId}");
        else if (action == "Delete")
            await ConfirmAndDeleteAsync(workoutId);
    }

    private async Task ConfirmAndDeleteAsync(Guid workoutId)
    {
        bool confirmed = await DisplayAlert("Delete Workout", "Are you sure you want to delete this workout?", "Delete", "Cancel");
        if (!confirmed) return;

        await _vm.DeleteWorkoutCommand.ExecuteAsync(workoutId);
        SessionCountLabel.Text = _vm.SessionCount;
        Calendar.MarkedDates = _vm.MarkedDates;
    }

    private void OnDateSelected(object sender, DateTime date)
    {
        _vm.SelectDateCommand.Execute(date);
        Calendar.SelectedDate = _vm.SelectedDate;
    }

    private void OnFilterTapped(object sender, TappedEventArgs e)
    {
        var filter = e.Parameter?.ToString() ?? "All";
        if (_activeFilter == filter) return;
        _activeFilter = filter;

        UpdateFilterChips(filter);

        var type = filter switch
        {
            "Strength" => WorkoutType.Strength,
            "Cardio" => WorkoutType.Cardio,
            "Stretch" => WorkoutType.Stretch,
            _ => WorkoutType.All,
        };
        _vm.SetFilterCommand.Execute(type);
    }

    private void UpdateFilterChips(string active)
    {
        var purple = Color.FromArgb("#7C3AED");
        var white = Colors.White;
        var gray = Color.FromArgb("#E5E7EB");
        var grayText = Color.FromArgb("#6B7280");

        var map = new Dictionary<string, (Border b, Label l)>
        {
            ["All"] = (FilterAll, (Label)FilterAll.Content),
            ["Strength"] = (FilterStrength, (Label)FilterStrength.Content),
            ["Cardio"] = (FilterCardio, (Label)FilterCardio.Content),
            ["Stretch"] = (FilterStretch, (Label)FilterStretch.Content),
        };

        foreach (var (key, (border, label)) in map)
        {
            bool isActive = key == active;
            border.BackgroundColor = isActive ? purple : white;
            border.Stroke = isActive ? purple : gray;
            border.StrokeThickness = isActive ? 0 : 1.5;
            label.TextColor = isActive ? white : grayText;
        }
    }
}
