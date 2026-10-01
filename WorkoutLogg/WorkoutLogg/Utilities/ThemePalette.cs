using Microsoft.Maui.Controls;

namespace WorkoutLogg.Utilities;

// Shared light/dark equivalents for authored colors, including dynamically built views.
public static class ThemePalette
{
    private static readonly Dictionary<string, (string Light, string Dark)> ColorsBySource = new(StringComparer.OrdinalIgnoreCase)
    {
        ["#EEEEF6"] = ("#F5F5F5", "#101010"),
        ["#F2F2F2"] = ("#F5F5F5", "#101010"),
        ["#FFFFFF"] = ("#FFFFFF", "#1C1C1C"),
        ["#F3F4F6"] = ("#EFEFEF", "#292929"),
        ["#F9FAFB"] = ("#EFEFEF", "#292929"),
        ["#F5F5F5"] = ("#EFEFEF", "#292929"),
        ["#111827"] = ("#171717", "#F5F5F5"),
        ["#374151"] = ("#171717", "#F5F5F5"),
        ["#000000"] = ("#171717", "#F5F5F5"),
        ["#6B7280"] = ("#525252", "#BDBDBD"),
        ["#59596A"] = ("#525252", "#BDBDBD"),
        ["#9CA3AF"] = ("#737373", "#A3A3A3"),
        ["#C0C0C0"] = ("#737373", "#A3A3A3"),
        ["#94969C"] = ("#737373", "#A3A3A3"),
        ["#B0A9A9"] = ("#737373", "#A3A3A3"),
        ["#C9D1DB"] = ("#737373", "#A3A3A3"),
        ["#E5E7EB"] = ("#DADADA", "#3D3D3D"),
        ["#D1D5DB"] = ("#DADADA", "#3D3D3D"),
        ["#CACDD9"] = ("#DADADA", "#3D3D3D"),
        ["#7C3AED"] = ("#B84408", "#FF975C"),
        ["#EA580C"] = ("#B84408", "#FF975C"),
        ["#C2410C"] = ("#B84408", "#FF975C"),
        ["#FF3300"] = ("#B84408", "#FF975C"),
        ["#EDE9FE"] = ("#FFF0E5", "#382419"),
        ["#F3F0FF"] = ("#FFF0E5", "#382419"),
        ["#FFF7ED"] = ("#FFF0E5", "#382419"),
        ["#9333EA"] = ("#333333", "#333333"),
        ["#DDD6FE"] = ("#D4D4D4", "#D4D4D4"),
        ["#C4B5FD"] = ("#B84408", "#FF975C"),
        ["#F0FDF4"] = ("#EAF7EF", "#193425"),
        ["#DCFCE7"] = ("#EAF7EF", "#193425"),
        ["#BBF7D0"] = ("#EAF7EF", "#193425"),
        ["#16A34A"] = ("#16713B", "#71D59A"),
        ["#15803D"] = ("#16713B", "#71D59A"),
        ["#22C55E"] = ("#16713B", "#71D59A"),
        ["#FEF2F2"] = ("#FFF0F0", "#3A2020"),
        ["#EF4444"] = ("#C52D36", "#FF8C94"),
        ["#DC2626"] = ("#C52D36", "#FF8C94"),
        ["#B91C1C"] = ("#C52D36", "#FF8C94"),
        ["#FCA5A5"] = ("#C52D36", "#FF8C94"),
        ["#EFF6FF"] = ("#EDF4FB", "#1C2C3C"),
        ["#DBEAFE"] = ("#EDF4FB", "#1C2C3C"),
        ["#E0F2FE"] = ("#EDF4FB", "#1C2C3C"),
        ["#2563EB"] = ("#2661A5", "#83B9F0"),
        ["#1D4ED8"] = ("#2661A5", "#83B9F0"),
        ["#0284C7"] = ("#2661A5", "#83B9F0"),
        ["#6B21A8"] = ("#7551A8", "#C3A4EE"),
        ["#FEF3C7"] = ("#FFF5D9", "#352D19"),
        ["#FEF9C3"] = ("#FFF5D9", "#352D19"),
        ["#92400E"] = ("#8D610F", "#F0C86A"),
        ["#A16207"] = ("#8D610F", "#F0C86A"),
        ["#F59E0B"] = ("#8D610F", "#F0C86A"),
        ["#FCE7F3"] = ("#FCEAF3", "#382432"),
        ["#BE185D"] = ("#A52A6B", "#ED9BC7"),
    };

    public static Color Resolve(Color source, bool dark, bool background = false)
    {
        var hex = source.ToHex().ToUpperInvariant();
        if (hex == "#FFFFFF") return background ? Color.FromArgb(dark ? "#1C1C1C" : "#FFFFFF").WithAlpha(source.Alpha) : source;
        if (hex == "#7C3AED" && background) return Color.FromArgb("#191919").WithAlpha(source.Alpha);
        return ColorsBySource.TryGetValue(hex, out var pair)
            ? Color.FromArgb(dark ? pair.Dark : pair.Light).WithAlpha(source.Alpha) : source;
    }
}
