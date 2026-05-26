using AKSoftware.Localization.MultiLanguages;
using System.Globalization;
using WorkoutLogg.Localization;

namespace WorkoutLogg.Services
{
    public class LanguageService
    {
        private const string PrefKey = "app_language";
        private readonly ILanguageContainerService _container;

        public LanguageService(ILanguageContainerService container)
        {
            _container = container;
        }

        /// <summary>Returns the saved language code, or "auto" if not set.</summary>
        public string CurrentCode => Preferences.Default.Get(PrefKey, "auto");

        /// <summary>Apply the user's saved preference (or detect from device).</summary>
        public void ApplyPreferred()
        {
            var code = CurrentCode;
            var culture = code == "auto" ? DetectDeviceCulture() : NormalizeCulture(code);
            ApplyCulture(culture);
        }

        // Normalize legacy short codes to full locale codes that match *.yml filenames.
        private static CultureInfo NormalizeCulture(string code) => code switch
        {
            "ru" => new CultureInfo("ru-RU"),
            "en" => new CultureInfo("en-US"),
            _ => new CultureInfo(code),
        };

        /// <summary>Switch language, persist choice, and reload the shell.</summary>
        public async Task SetLanguageAsync(string langCode)
        {
            Preferences.Default.Set(PrefKey, langCode);
            var culture = langCode == "auto" ? DetectDeviceCulture() : NormalizeCulture(langCode);
            ApplyCulture(culture);
            await ReloadShellAsync();
        }

        private void ApplyCulture(CultureInfo culture)
        {
            _container.SetLanguage(culture);
            // Keep Loc static in sync
            Loc.Initialize(_container);
        }

        private static CultureInfo DetectDeviceCulture()
        {
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return NormalizeCulture(lang);
        }

        private static Task ReloadShellAsync() =>
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var services = IPlatformApplication.Current!.Services;
                var shell = services.GetRequiredService<AppShell>();
                Application.Current!.Windows[0].Page = shell;
                await shell.GoToAsync("//Dashboard");
            });
    }
}
