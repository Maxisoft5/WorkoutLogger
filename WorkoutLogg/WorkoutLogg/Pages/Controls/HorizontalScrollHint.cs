using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using WorkoutLogg.Localization;

namespace WorkoutLogg.Pages.Controls;

// Индикатор занимает отдельную строку: он не закрывает крайний chip и не ловит свайпы.
[ContentProperty(nameof(ScrollContent))]
public sealed class HorizontalScrollHint : ContentView
{
    private readonly ScrollView _scroll = new()
    {
        Orientation = ScrollOrientation.Horizontal,
        HorizontalScrollBarVisibility = ScrollBarVisibility.Never
    };
    private readonly HorizontalOverflowIndicator _indicator = new();

    public static readonly BindableProperty ScrollContentProperty = BindableProperty.Create(
        nameof(ScrollContent), typeof(View), typeof(HorizontalScrollHint), null,
        propertyChanged: (b, oldValue, newValue) => ((HorizontalScrollHint)b).SetScrollContent((View?)oldValue, (View?)newValue));

    public View? ScrollContent
    {
        get => (View?)GetValue(ScrollContentProperty);
        set => SetValue(ScrollContentProperty, value);
    }

    public HorizontalScrollHint()
    {
        var root = new Grid { RowDefinitions = new RowDefinitionCollection(new(GridLength.Auto), new(GridLength.Auto)) };
        root.Add(_scroll);
        root.Add(_indicator, 0, 1);
        Content = root;
        _scroll.Scrolled += (_, _) => UpdateIndicator();
        _scroll.SizeChanged += (_, _) => UpdateIndicator();
        _scroll.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(ScrollView.ContentSize)) UpdateIndicator(); };
        Loaded += (_, _) => UpdateIndicator();
    }

    private void SetScrollContent(View? oldView, View? newView)
    {
        if (oldView is not null) oldView.SizeChanged -= OnContentSizeChanged;
        _scroll.Content = newView;
        if (newView is not null) newView.SizeChanged += OnContentSizeChanged;
        UpdateIndicator();
    }

    private void OnContentSizeChanged(object? sender, EventArgs e) => UpdateIndicator();
    private void UpdateIndicator() => _indicator.Update(_scroll.Width, _scroll.ContentSize.Width, _scroll.ScrollX);
}

// Сохраняет виртуализацию CollectionView. Размер карточки и промежуток задаются явно.
[ContentProperty(nameof(Collection))]
public sealed class HorizontalCollectionHint : ContentView
{
    private readonly Grid _root = new() { RowDefinitions = new RowDefinitionCollection(new(GridLength.Auto), new(GridLength.Auto)) };
    private readonly HorizontalOverflowIndicator _indicator = new();
    private CollectionView? _collection;
    private INotifyCollectionChanged? _items;
    private double _offset;

    public double ItemWidth { get; set; } = 220;
    public double ItemSpacing { get; set; } = 12;

    public CollectionView? Collection
    {
        get => _collection;
        set
        {
            DetachItems();
            if (_collection is not null)
            {
                _collection.Scrolled -= OnScrolled;
                _collection.SizeChanged -= OnSizeChanged;
                _collection.PropertyChanged -= OnCollectionPropertyChanged;
                _root.Remove(_collection);
            }
            _collection = value;
            _offset = 0;
            if (value is not null)
            {
                _root.Add(value);
                value.Scrolled += OnScrolled;
                value.SizeChanged += OnSizeChanged;
                value.PropertyChanged += OnCollectionPropertyChanged;
            }
            AttachItems();
            UpdateIndicator();
        }
    }

    public HorizontalCollectionHint()
    {
        _root.Add(_indicator, 0, 1);
        Content = _root;
        Loaded += (_, _) => { AttachItems(); UpdateIndicator(); };
        Unloaded += (_, _) => DetachItems();
    }

    private void OnScrolled(object? sender, ItemsViewScrolledEventArgs e) { _offset = e.HorizontalOffset; UpdateIndicator(); }
    private void OnSizeChanged(object? sender, EventArgs e) => UpdateIndicator();
    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => UpdateIndicator();
    private void OnCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CollectionView.ItemsSource)) { AttachItems(); UpdateIndicator(); }
    }
    private void AttachItems()
    {
        DetachItems();
        if (!IsLoaded) return;
        _items = _collection?.ItemsSource as INotifyCollectionChanged;
        if (_items is not null) _items.CollectionChanged += OnItemsChanged;
    }
    private void DetachItems()
    {
        if (_items is not null) _items.CollectionChanged -= OnItemsChanged;
        _items = null;
    }
    private void UpdateIndicator()
    {
        var count = _collection?.ItemsSource is ICollection items
            ? items.Count : _collection?.ItemsSource?.Cast<object>().Count() ?? 0;
        var extent = count * ItemWidth + Math.Max(0, count - 1) * ItemSpacing;
        _indicator.Update(_collection?.Width ?? 0, extent, _offset);
    }
}

internal sealed class HorizontalOverflowIndicator : GraphicsView, IDrawable
{
    private double _viewport;
    private double _extent;
    private double _offset;

    public HorizontalOverflowIndicator()
    {
        HeightRequest = 14;
        InputTransparent = true;
        IsVisible = false;
        Drawable = this;
        SemanticProperties.SetDescription(this, Loc.Get("Common_HorizontalScrollHint"));
    }

    public void Update(double viewport, double extent, double offset)
    {
        _viewport = Math.Max(0, viewport);
        _extent = Math.Max(0, extent);
        _offset = Math.Clamp(offset, 0, Math.Max(0, _extent - _viewport));
        IsVisible = _viewport > 0 && _extent > _viewport + 1;
        Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_extent <= _viewport || dirtyRect.Width < 40) return;
        var trackWidth = dirtyRect.Width - 32;
        var thumbWidth = Math.Min(trackWidth, Math.Max(20, trackWidth * (float)(_viewport / _extent)));
        var progress = (float)(_offset / (_extent - _viewport));
        canvas.FillColor = Application.Current?.RequestedTheme == AppTheme.Dark
            ? Color.FromArgb("#3D3D3D") : Color.FromArgb("#DADADA");
        canvas.FillRoundedRectangle(16, 6, trackWidth, 3, 1.5f);
        canvas.FillColor = Color.FromArgb("#B84408");
        canvas.FillRoundedRectangle(16 + progress * (trackWidth - thumbWidth), 6, thumbWidth, 3, 1.5f);
        canvas.StrokeColor = Color.FromArgb("#B84408");
        canvas.StrokeSize = 1.5f;
        if (_offset > 1)
        {
            canvas.DrawLine(8, 3, 4, 7);
            canvas.DrawLine(4, 7, 8, 11);
        }
        if (_offset < _extent - _viewport - 1)
        {
            var x = dirtyRect.Width - 8;
            canvas.DrawLine(x, 3, x + 4, 7);
            canvas.DrawLine(x + 4, 7, x, 11);
        }
    }
}
