using WorkoutLogg.Utilities;
using System.Globalization;
using WorkoutLogg.Localization;

namespace WorkoutLogg.Pages.Controls;

public partial class CalendarPicker : ContentView
{
    // ── Bindable Properties ───────────────────────────────────────────────

    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(CalendarPicker), DateTime.Today,
            propertyChanged: (b, _, _) => ((CalendarPicker)b).RequestRebuild());

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>Даты с точкой-индикатором тренировки</summary>
    public static readonly BindableProperty MarkedDatesProperty =
        BindableProperty.Create(nameof(MarkedDates), typeof(IEnumerable<DateTime>), typeof(CalendarPicker), null,
            propertyChanged: (b, _, _) => ((CalendarPicker)b).RequestRebuild());

    public IEnumerable<DateTime> MarkedDates
    {
        get => (IEnumerable<DateTime>)GetValue(MarkedDatesProperty);
        set => SetValue(MarkedDatesProperty, value);
    }

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(CalendarPicker), Color.FromArgb("#7C3AED"),
            propertyChanged: (b, _, _) => ((CalendarPicker)b).RequestRebuild());

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    // ── Event ─────────────────────────────────────────────────────────────
    public event EventHandler<DateTime>? DateSelected;

    // ── State ─────────────────────────────────────────────────────────────
    private DateTime _viewMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private DateTime? _renderedMonth;
    private bool _rebuildQueued;
    private readonly List<(DateTime Date, Border Background, Label Number,
        Microsoft.Maui.Controls.Shapes.Ellipse Dot)> _cells = [];

    public CalendarPicker()
    {
        InitializeComponent();
        // Создание ячеек откладывается до загрузки контрола. Изменения свойств
        // при разборе XAML не должны несколько раз строить один календарь.
        Loaded += (_, _) => RequestRebuild();
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
        if (!IsLoaded || MonthLabel == null || DaysGrid == null) return;

        MonthLabel.Text = _viewMonth.ToString("MMMM yyyy", new CultureInfo(Loc.Get("_Culture")));

        var marked = MarkedDates?.Select(d => d.Date).ToHashSet() ?? [];
        var selected = SelectedDate?.Date;
        var today = DateTime.Today;

        if (_renderedMonth == _viewMonth)
        {
            // При смене выделения / меток переиспользуем native views.
            foreach (var cell in _cells)
            {
                bool isSelected = cell.Date == selected;
                bool isToday = cell.Date == today;
                cell.Background.WithThemeColor("BackgroundColor", isSelected ? AccentColor
                    : isToday ? Color.FromArgb("#EDE9FE") : Colors.Transparent);
                cell.Number.FontAttributes = isSelected || isToday ? FontAttributes.Bold : FontAttributes.None;
                cell.Number.WithThemeColor("TextColor", isSelected ? Colors.White
                    : isToday ? AccentColor : Color.FromArgb("#111827"));
                cell.Dot.WithThemeColor("Fill", isSelected ? Colors.White : AccentColor);
                cell.Dot.IsVisible = marked.Contains(cell.Date);
            }
            return;
        }

        DaysGrid.Children.Clear();
        _cells.Clear();
        _renderedMonth = _viewMonth;

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
            DaysGrid.Children.Add(cell);

            if (++col > 6) { col = 0; row++; }
        }
    }

    private void RequestRebuild()
    {
        if (!IsLoaded || _rebuildQueued)
            return;

        _rebuildQueued = true;
        Dispatcher.Dispatch(() =>
        {
            _rebuildQueued = false;
            Rebuild();
        });
    }

    private View BuildCell(int day, bool isSel, bool isTod, bool isMark, DateTime date)
    {
        var accent = AccentColor;
        var accentLight = Color.FromArgb("#EDE9FE");

        // Размер квадрата-индикатора выделения (не самой ячейки)
        double bgSize = DeviceInfo.Idiom == DeviceIdiom.Desktop ? 44 : 38;

        // Индикатор выделения — фиксированный квадрат, наложен поверх
        var selBg = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            HeightRequest = bgSize,
            WidthRequest = bgSize,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        }.WithThemeColor("BackgroundColor", isSel ? accent : isTod ? accentLight : Colors.Transparent);

        // Число — свободно центрируется в ячейке, не зажато в bgSize
        var label = new Label
        {
            Text = day.ToString(),
            FontSize = DeviceInfo.Idiom == DeviceIdiom.Desktop ? 15 : 14,
            FontAttributes = isSel || isTod ? FontAttributes.Bold : FontAttributes.None,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        }.WithThemeColor("TextColor", isSel ? Colors.White : isTod ? accent : Color.FromArgb("#111827"));

        // Точка-индикатор залоггированного дня
        var dot = new Microsoft.Maui.Controls.Shapes.Ellipse
        {
            HeightRequest = 5,
            WidthRequest = 5,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = isMark
        }.WithThemeColor("Fill", isSel ? Colors.White : accent);

        // Контейнер заполняет всю колонку; selBg и label в одной строке (overlay)
        var container = new Grid
        {
            MinimumHeightRequest = 48,
            HorizontalOptions = LayoutOptions.Fill,
            RowDefinitions = new RowDefinitionCollection(
                new RowDefinition(new GridLength(bgSize)),
                new RowDefinition(new GridLength(8))),
        };

        Grid.SetRow(selBg, 0);
        Grid.SetRow(label, 0);
        Grid.SetRow(dot, 1);

        container.Children.Add(selBg);
        container.Children.Add(label);
        container.Children.Add(dot);
        _cells.Add((date, selBg, label, dot));

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) =>
        {
            SelectedDate = date;
            DateSelected?.Invoke(this, date);
        };
        container.GestureRecognizers.Add(tap);

        return container;
    }
}
