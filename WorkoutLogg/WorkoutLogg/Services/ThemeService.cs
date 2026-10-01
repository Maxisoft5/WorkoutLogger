namespace WorkoutLogg.Services;

public static class ThemeService
{
    private const string PreferenceKey = "appearance.theme";

    public static string Selected => Preferences.Default.Get(PreferenceKey, "system");

    public static void Initialize()
    {
        if (Application.Current is { } app)
            app.RequestedThemeChanged += (_, _) => UpdateSystemBars();
        Apply(Selected);
    }

    public static void Set(string choice)
    {
        if (choice is not ("light" or "dark" or "system")) return;
        Preferences.Default.Set(PreferenceKey, choice);
        Apply(choice);
    }

    private static void Apply(string choice)
    {
        if (Application.Current is { } app)
            app.UserAppTheme = choice switch
            {
                "light" => AppTheme.Light,
                "dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        UpdateSystemBars();
    }

    public static void UpdateSystemBars()
    {
#if ANDROID
        if (Platform.CurrentActivity?.Window is not { } window) return;
        var dark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var color = Android.Graphics.Color.ParseColor(dark ? "#101010" : "#F5F5F5");
#pragma warning disable CA1422, CS0618 // Android versions before enforced edge-to-edge use these window colors.
        window.SetStatusBarColor(color);
        window.SetNavigationBarColor(color);
        var flags = (Android.Views.SystemUiFlags)window.DecorView.SystemUiVisibility;
        flags = dark ? flags & ~Android.Views.SystemUiFlags.LightStatusBar
            : flags | Android.Views.SystemUiFlags.LightStatusBar;
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            flags = dark ? flags & ~Android.Views.SystemUiFlags.LightNavigationBar
                : flags | Android.Views.SystemUiFlags.LightNavigationBar;
        window.DecorView.SystemUiVisibility = (Android.Views.StatusBarVisibility)flags;
#pragma warning restore CA1422, CS0618
#endif
    }
}
