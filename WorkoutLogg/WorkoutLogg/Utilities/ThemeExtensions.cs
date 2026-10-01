using Microsoft.Maui.Controls.Shapes;

namespace WorkoutLogg.Utilities;

public static class ThemeExtensions
{
    // Color and Brush properties share the same palette; bindings update existing views in place.
    public static T WithThemeColor<T>(this T view, string propertyName, object? value) where T : BindableObject
    {
        // Explicit property references also survive trimming in Release builds.
        var property = (view, propertyName) switch
        {
            (VisualElement, "BackgroundColor") => VisualElement.BackgroundColorProperty,
            (Label, "TextColor") => Label.TextColorProperty,
            (Span, "TextColor") => Span.TextColorProperty,
            (Button, "TextColor") => Button.TextColorProperty,
            (InputView, "TextColor") => InputView.TextColorProperty,
            (InputView, "PlaceholderColor") => InputView.PlaceholderColorProperty,
            (Picker, "TextColor") => Picker.TextColorProperty,
            (Picker, "TitleColor") => Picker.TitleColorProperty,
            (DatePicker, "TextColor") => DatePicker.TextColorProperty,
            (TimePicker, "TextColor") => TimePicker.TextColorProperty,
            (Border, "Stroke") => Border.StrokeProperty,
            (Shape, "Stroke") => Shape.StrokeProperty,
            (Shape, "Fill") => Shape.FillProperty,
            (FontImageSource, "Color") => FontImageSource.ColorProperty,
            (BoxView, "Color") => BoxView.ColorProperty,
            (ActivityIndicator, "Color") => ActivityIndicator.ColorProperty,
            _ => throw new InvalidOperationException($"Unsupported theme property {view.GetType().Name}.{propertyName}.")
        };
        var color = value switch { Color c => c, SolidColorBrush b => b.Color, _ => null };
        if (color is null)
        {
            view.SetValue(property, value);
            return view;
        }
        var background = propertyName is "BackgroundColor" or "Background";
        var light = ThemePalette.Resolve(color, false, background);
        var dark = ThemePalette.Resolve(color, true, background);
        if (typeof(Brush).IsAssignableFrom(property.ReturnType))
            view.SetAppTheme<Brush>(property, new SolidColorBrush(light), new SolidColorBrush(dark));
        else
            view.SetAppThemeColor(property, light, dark);
        return view;
    }
}
