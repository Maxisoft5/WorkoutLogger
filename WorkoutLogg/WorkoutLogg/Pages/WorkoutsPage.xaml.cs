namespace WorkoutLogg.Pages;

public partial class WorkoutsPage : ContentPage
{
    private string _activeFilter = "All";

    public WorkoutsPage()
    {
        InitializeComponent();
    }

    private void OnDateSelected(object sender, DateTime date)
    {
        // date — выбранная дата, фильтруй список тренировок
    }

    private void OnFilterTapped(object sender, TappedEventArgs e)
    {
        var filter = e.Parameter?.ToString() ?? "All";
        if (_activeFilter == filter) return;
        _activeFilter = filter;

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
            bool active = key == filter;
            border.BackgroundColor = active ? purple : white;
            border.Stroke = active ? purple : gray;
            border.StrokeThickness = active ? 0 : 1.5;
            label.TextColor = active ? white : grayText;
        }

        // TODO: фильтровать список тренировок по категории
    }
}