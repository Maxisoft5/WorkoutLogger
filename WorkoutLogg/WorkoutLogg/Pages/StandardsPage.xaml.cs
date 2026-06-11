using WorkoutLogg.Localization;

namespace WorkoutLogg.Pages;

public partial class StandardsPage : ContentPage
{
    // rank → (squat, bench, deadlift)
    private record LiftStd(int Squat, int Bench, int Deadlift);

    // weight class → rank list [3rd, 2nd, 1st, KMS, MS]
    private static readonly Dictionary<string, LiftStd[]> MenStandards = new()
    {
        ["59"] =  [new(75,55,95),  new(105,75,130), new(135,95,165),  new(170,120,210), new(205,145,250)],
        ["66"] =  [new(85,60,105), new(115,85,145), new(150,110,185), new(190,140,235), new(230,170,285)],
        ["74"] =  [new(95,70,120), new(130,95,165), new(170,120,210), new(215,150,265), new(260,185,320)],
        ["83"] =  [new(110,75,130),new(145,105,185),new(190,135,235), new(240,170,295), new(290,205,355)],
        ["93"] =  [new(120,85,145),new(160,115,200),new(210,150,260), new(265,190,330), new(320,230,400)],
        ["105"] = [new(130,90,160),new(175,125,220),new(230,165,285), new(290,205,360), new(350,250,430)],
        ["120"] = [new(140,100,175),new(190,135,240),new(250,175,310),new(315,220,390), new(380,270,470)],
        ["120+"]= [new(150,105,185),new(200,145,260),new(265,190,335),new(335,235,420), new(405,285,505)],
    };

    private static readonly Dictionary<string, LiftStd[]> WomenStandards = new()
    {
        ["47"] =  [new(45,30,55),  new(62,42,75),  new(80,55,97),   new(102,70,123),  new(125,85,150)],
        ["52"] =  [new(50,35,62),  new(70,47,85),  new(90,62,110),  new(115,78,138),  new(140,95,170)],
        ["57"] =  [new(57,38,70),  new(77,53,95),  new(100,68,122), new(127,87,155),  new(155,105,190)],
        ["63"] =  [new(62,42,77),  new(85,57,104), new(110,74,135), new(140,93,170),  new(170,114,210)],
        ["69"] =  [new(68,46,84),  new(93,62,114), new(120,80,148), new(152,102,188), new(185,124,230)],
        ["76"] =  [new(73,50,90),  new(100,67,122),new(130,87,160), new(164,110,202), new(200,133,247)],
        ["84"] =  [new(80,54,98),  new(108,73,132),new(140,94,172), new(177,119,218), new(215,145,265)],
        ["84+"]= [new(87,59,107), new(118,80,145),new(153,103,188),new(193,130,238),  new(235,158,290)],
    };

    private static readonly string[] RankNames = ["III", "II", "I", "KMC", "MC"];
    private static readonly Color[] RankColors =
    [
        Color.FromArgb("#F9FAFB"),
        Color.FromArgb("#F0FDF4"),
        Color.FromArgb("#EFF6FF"),
        Color.FromArgb("#FFF7ED"),
        Color.FromArgb("#FEF2F2"),
    ];
    private static readonly Color[] RankTextColors =
    [
        Color.FromArgb("#374151"),
        Color.FromArgb("#15803D"),
        Color.FromArgb("#1D4ED8"),
        Color.FromArgb("#C2410C"),
        Color.FromArgb("#B91C1C"),
    ];

    private bool _isMen = true;
    private string _selectedWeight = "83";

    public StandardsPage()
    {
        InitializeComponent();
        BuildWeightChips(MenStandards.Keys.ToList());
        RenderTable();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private void OnMenTapped(object sender, TappedEventArgs e)
    {
        if (_isMen) return;
        _isMen = true;
        _selectedWeight = "83";
        UpdateSexToggle();
        BuildWeightChips(MenStandards.Keys.ToList());
        RenderTable();
    }

    private void OnWomenTapped(object sender, TappedEventArgs e)
    {
        if (!_isMen) return;
        _isMen = false;
        _selectedWeight = "63";
        UpdateSexToggle();
        BuildWeightChips(WomenStandards.Keys.ToList());
        RenderTable();
    }

    private void UpdateSexToggle()
    {
        MenBorder.BackgroundColor    = _isMen ? Color.FromArgb("#7C3AED") : Colors.White;
        MenBorder.Stroke            = _isMen ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Color.FromArgb("#E5E7EB"));
        MenBorder.StrokeThickness   = _isMen ? 0 : 1.5;
        MenLabel.TextColor          = _isMen ? Colors.White : Color.FromArgb("#374151");

        WomenBorder.BackgroundColor  = _isMen ? Colors.White : Color.FromArgb("#7C3AED");
        WomenBorder.Stroke          = _isMen ? new SolidColorBrush(Color.FromArgb("#E5E7EB")) : new SolidColorBrush(Colors.Transparent);
        WomenBorder.StrokeThickness = _isMen ? 1.5 : 0;
        WomenLabel.TextColor        = _isMen ? Color.FromArgb("#374151") : Colors.White;
    }

    private void BuildWeightChips(List<string> keys)
    {
        WeightChipsStack.Children.Clear();
        if (!keys.Contains(_selectedWeight))
            _selectedWeight = keys[keys.Count > 4 ? 4 : 0];

        foreach (var key in keys)
        {
            var isSelected = key == _selectedWeight;
            var chip = new Border
            {
                BackgroundColor = isSelected ? Color.FromArgb("#7C3AED") : Colors.White,
                Stroke = isSelected ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Color.FromArgb("#E5E7EB")),
                StrokeThickness = isSelected ? 0 : 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                Padding = new Thickness(14, 6),
            };
            var label = new Label
            {
                Text = $"{key} {Loc.Get("Common_Kg")}",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = isSelected ? Colors.White : Color.FromArgb("#374151"),
            };
            chip.Content = label;

            var capturedKey = key;
            chip.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    _selectedWeight = capturedKey;
                    BuildWeightChips(keys);
                    RenderTable();
                })
            });

            WeightChipsStack.Children.Add(chip);
        }
    }

    private void RenderTable()
    {
        TableRows.Children.Clear();

        var standards = (_isMen ? MenStandards : WomenStandards);
        if (!standards.TryGetValue(_selectedWeight, out var rows)) return;

        for (int i = 0; i < rows.Length; i++)
        {
            var std = rows[i];
            var bg = RankColors[Math.Min(i, RankColors.Length - 1)];
            var fg = RankTextColors[Math.Min(i, RankTextColors.Length - 1)];
            var total = std.Squat + std.Bench + std.Deadlift;

            var row = new Border
            {
                BackgroundColor = bg,
                Padding = new Thickness(12, 10),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                StrokeThickness = 0,
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star))
            };

            grid.Add(MakeCell(RankNames[i], FontAttributes.Bold, fg), 0, 0);
            grid.Add(MakeCell($"{std.Squat}", FontAttributes.None, Color.FromArgb("#111827")), 1, 0);
            grid.Add(MakeCell($"{std.Bench}", FontAttributes.None, Color.FromArgb("#111827")), 2, 0);
            grid.Add(MakeCell($"{std.Deadlift}", FontAttributes.None, Color.FromArgb("#111827")), 3, 0);
            grid.Add(MakeCell($"{total}", FontAttributes.Bold, fg), 4, 0);

            row.Content = grid;
            TableRows.Children.Add(row);
        }
    }

    private static Label MakeCell(string text, FontAttributes attrs, Color color) => new()
    {
        Text = text,
        FontSize = 13,
        FontAttributes = attrs,
        TextColor = color,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
    };
}
