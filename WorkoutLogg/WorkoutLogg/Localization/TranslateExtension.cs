using Microsoft.Maui.Controls.Xaml;

namespace WorkoutLogg.Localization
{
    /// <summary>
    /// XAML markup extension: Text="{loc:Translate Dashboard_Title}"
    /// Reads the value at XAML inflation time — the shell is reloaded on language change
    /// so all markup extensions re-evaluate automatically.
    /// </summary>
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension<string>
    {
        public string Key { get; set; } = "";

        public string ProvideValue(IServiceProvider _) => Loc.Get(Key);

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);
    }
}
