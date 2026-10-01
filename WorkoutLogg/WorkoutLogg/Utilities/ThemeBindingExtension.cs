using System.ComponentModel;
using System.Globalization;
using Microsoft.Maui.Controls.Xaml;

namespace WorkoutLogg.Utilities;

// Keeps model-supplied badge, record and status colors responsive to theme changes.
[ContentProperty(nameof(Path))]
public sealed class ThemeBindingExtension : IMarkupExtension<BindingBase>
{
    public string Path { get; set; } = ".";
    public bool Background { get; set; }

    public BindingBase ProvideValue(IServiceProvider serviceProvider) => new MultiBinding
    {
        Bindings = { new Binding(Path), new Binding(nameof(ThemeState.IsDark), source: ThemeState.Instance) },
        Converter = new PaletteConverter(Background)
    };

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    private sealed class PaletteConverter(bool background) : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] is not Color color || values[1] is not bool dark)
                return BindableProperty.UnsetValue;
            var result = ThemePalette.Resolve(color, dark, background);
            return typeof(Brush).IsAssignableFrom(targetType) ? new SolidColorBrush(result) : result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}

public sealed class ThemeState : INotifyPropertyChanged
{
    public static ThemeState Instance { get; } = new();
    public bool IsDark => Application.Current?.RequestedTheme == AppTheme.Dark;
    public event PropertyChangedEventHandler? PropertyChanged;

    private ThemeState()
    {
        if (Application.Current is { } app)
            app.RequestedThemeChanged += (_, _) => PropertyChanged?.Invoke(this, new(nameof(IsDark)));
    }
}
