using AKSoftware.Localization.MultiLanguages;

namespace WorkoutLogg.Localization
{
    /// <summary>Static accessor — initialized once from MauiProgram after DI build.</summary>
    public static class Loc
    {
        private static ILanguageContainerService? _service;

        public static void Initialize(ILanguageContainerService service) => _service = service;

        /// <summary>Returns the translated string for <paramref name="key"/>, or the key itself if missing.</summary>
        public static string Get(string key)
        {
            if (_service is null) return key;
            var value = _service.Keys[key];
            return string.IsNullOrEmpty(value) ? key : value;
        }
    }
}
