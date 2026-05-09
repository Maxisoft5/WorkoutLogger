namespace WorkoutLogg.Pages.Controls;

public partial class CalendarPicker : ContentView
{
    // ── Bindable Properties ───────────────────────────────────────────────

    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(CalendarPicker), null,
            propertyChanged: (b, _, _) => ((CalendarPicker)b).Rebuild());

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>Даты с точкой-индикатором тренировки</summary>
    public static readonly BindableProperty MarkedDatesProperty =
        BindableProperty.Create(nameof(MarkedDates), typeof(IEnumerable<DateTime>), typeof(CalendarPicker), null,
            propertyChanged: (b, _, _) => ((CalendarPicker)b).Rebuild());

    public IEnumerable<DateTime> MarkedDates
    {
        get => (IEnumerable<DateTime>)GetValue(MarkedDatesProperty);
        set => SetValue(MarkedDatesProperty, value);
    }

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(CalendarPicker), Color.FromArgb("#7C3AED"),
            propertyChanged: (b, _, _) => ((CalendarPicker)b).Rebuild());

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    // ── Event ─────────────────────────────────────────────────────────────
    public event EventHandler<DateTime>? DateSelected;

    // ── State ─────────────────────────────────────────────────────────────
    private DateTime _viewMonth;

    public CalendarPicker()
    {
        InitializeComponent();
        _viewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        Rebuild();
    }

    private void OnPrevMonth(object sender, EventArgs e)
    {
        _viewMonth = _viewMonth.AddMonths(-1);
        Rebuild();
    }

    private void OnNextMonth(object sender, EventArgs e)
    {
        _viewMonth = _viewMonth.AddMonths(1);
        Rebuild();
    }

    private void Rebuild()
    {
        if (MonthLabel == null || DaysGrid == null) return;

        MonthLabel.Text = _viewMonth.ToString("MMMM yyyy");
        DaysGrid.Children.Clear();

        var marked = MarkedDates?.Select(d => d.Date).ToHashSet() ?? [];
        var selected = SelectedDate?.Date;
        var today = DateTime.Today;

        // ISO week: Monday = 0
        int firstDow = ((int)_viewMonth.DayOfWeek + 6) % 7;
        int daysInMonth = DateTime.DaysInMonth(_viewMonth.Year, _viewMonth.Month);

        int col = firstDow;
        int row = 0;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(_viewMonth.Year, _viewMonth.Month, day);
            bool isSel = selected.HasValue && date == selected.Value;
            bool isTod = date == today;
            bool isMark = marked.Contains(date);

            var cell = BuildCell(day, isSel, isTod, isMark, date);
            Grid.SetColumn(cell, col);
            Grid.SetRow(cell, row);
            cell.HorizontalOptions = LayoutOptions.Center;
            cell.VerticalOptions = LayoutOptions.Center;
            DaysGrid.Children.Add(cell);

            if (++col > 6) { col = 0; row++; }
        }
    }

    private View BuildCell(int day, bool isSel, bool isTod, bool isMark, DateTime date)
    {
        var accent = AccentColor;
        var accentLight = Color.FromArgb("#EDE9FE");

        var label = new Label
        {
            Text = day.ToString(),
            FontSize = 14,
            FontAttributes = isSel || isTod ? FontAttributes.Bold : FontAttributes.None,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            TextColor = isSel ? Colors.White
                              : isTod ? accent
                              : Color.FromArgb("#111827"),
        };

        var dot = new Microsoft.Maui.Controls.Shapes.Ellipse
        {
            Fill = isSel ? Colors.White : accent,
            HeightRequest = 15,
            WidthRequest = 15,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 2, 0, 0),
            IsVisible = isMark,
        };

        // Grid вместо VerticalStackLayout — он всегда занимает всю ячейку
        var inner = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
        {
            new RowDefinition { Height = GridLength.Star },
            new RowDefinition { Height = 8 },
        },
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 50,
            HeightRequest = 50,
        };
        Grid.SetRow(label, 0);
        Grid.SetRow(dot, 1);
        inner.Children.Add(label);
        inner.Children.Add(dot);

        var bg = new Border
        {
            BackgroundColor = isSel ? accent : isTod ? accentLight : Colors.Transparent,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            HeightRequest = 100,
            WidthRequest = 100,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = inner,
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) =>
        {
            SelectedDate = date;
            DateSelected?.Invoke(this, date);
        };
        bg.GestureRecognizers.Add(tap);

        return bg;
    }
}