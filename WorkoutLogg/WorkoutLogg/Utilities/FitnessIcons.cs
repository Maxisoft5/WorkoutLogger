namespace WorkoutLogg.Utilities;

// Семантический набор приложения. Все знаки берутся из одного встроенного шрифта.
public static class FitnessIcons
{
    public const string FontFamily = FluentUI.FontFamily;
    public const string Brand = FluentUI.heart_pulse_24_regular;
    public const string Strength = FluentUI.dumbbell_24_regular;
    public const string Cardio = FluentUI.pulse_24_regular;
    public const string Mobility = FluentUI.accessibility_24_regular;
    public const string Coach = FluentUI.sparkle_24_regular;
    public const string Profile = FluentUI.person_24_regular;
    public const string Calendar = FluentUI.calendar_ltr_24_regular;
    public const string Journal = FluentUI.clipboard_task_list_ltr_24_regular;
    public const string Fire = FluentUI.fire_24_regular;
    public const string Trophy = FluentUI.trophy_24_regular;
    public const string Star = FluentUI.star_24_regular;
    public const string Target = FluentUI.target_24_regular;
    public const string Premium = FluentUI.premium_24_regular;
    public const string Chart = FluentUI.chart_multiple_24_regular;
    public const string Progress = FluentUI.arrow_trending_lines_24_regular;
    public const string Timer = FluentUI.timer_24_regular;
    public const string Wallet = FluentUI.wallet_24_regular;
    public const string Payment = FluentUI.payment_24_regular;
    public const string Bank = FluentUI.building_bank_24_regular;
    public const string Phone = FluentUI.phone_24_regular;
    public const string People = FluentUI.people_24_regular;
    public const string Chat = FluentUI.chat_24_regular;
    public const string Mail = FluentUI.mail_24_regular;
    public const string Key = FluentUI.key_24_regular;
    public const string Lock = FluentUI.lock_closed_24_regular;
    public const string Shield = FluentUI.shield_24_regular;
    public const string Settings = FluentUI.settings_24_regular;
    public const string Document = FluentUI.document_24_regular;
    public const string Legal = FluentUI.gavel_24_regular;
    public const string Link = FluentUI.link_24_regular;
    public const string Search = FluentUI.search_24_regular;
    public const string CheckCircle = FluentUI.checkmark_circle_24_regular;
    public const string Check = FluentUI.checkmark_24_regular;
    public const string Circle = FluentUI.circle_24_regular;
    public const string Close = FluentUI.dismiss_24_regular;
    public const string Add = FluentUI.add_24_regular;
    public const string Back = FluentUI.arrow_left_24_regular;
    public const string Up = FluentUI.chevron_up_24_regular;
    public const string Down = FluentUI.chevron_down_24_regular;
    public const string Left = FluentUI.chevron_left_24_regular;
    public const string Right = FluentUI.chevron_right_24_regular;
    public const string Send = FluentUI.arrow_up_24_regular;
    public const string More = FluentUI.more_vertical_24_regular;
    public const string Edit = FluentUI.edit_24_regular;
    public const string Prohibited = FluentUI.prohibited_24_regular;

    public static Label Action(string glyph, string description, Color? color = null)
    {
        var label = new Label
        {
            Text = glyph, FontFamily = FontFamily, FontSize = 22,
            FontAutoScalingEnabled = false,
            BackgroundColor = Colors.Transparent,
            MinimumWidthRequest = 48, MinimumHeightRequest = 48,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        }.WithThemeColor("TextColor", color ?? Color.FromArgb("#7C3AED"));
        SemanticProperties.SetDescription(label, description);
        return label;
    }
}
