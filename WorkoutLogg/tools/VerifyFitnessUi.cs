// Запуск из папки решения: dotnet run --file tools/VerifyFitnessUi.cs
// Проверяет реальные глифы TTF и XAML-ресурсы без установки приложения на телефон.
using System.Buffers.Binary;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var app = Path.Combine(Directory.GetCurrentDirectory(), "WorkoutLogg");
var fluent = File.ReadAllText(Path.Combine(app, "Resources/Fonts/FluentUI.cs"));
var glyphs = Regex.Matches(fluent, "public const string (\\w+) = \"\\\\[uU]([0-9a-fA-F]+)\";")
    .ToDictionary(m => m.Groups[1].Value, m => Convert.ToUInt32(m.Groups[2].Value, 16));
var aliases = Regex.Matches(File.ReadAllText(Path.Combine(app, "Utilities/FitnessIcons.cs")),
    @"public const string (\w+) = FluentUI\.(\w+);");
var font = File.ReadAllBytes(Path.Combine(app, "Resources/Fonts/FluentSystemIcons-Regular.ttf"));
foreach (Match alias in aliases)
{
    if (alias.Groups[1].Value == "FontFamily") continue;
    var name = alias.Groups[2].Value;
    Require(glyphs.TryGetValue(name, out var codepoint), $"Неизвестная константа: {name}");
    Require(HasGlyph(font, codepoint), $"Глиф {name} отсутствует в поставляемом TTF");
}

XNamespace x = "http://schemas.microsoft.com/winfx/2009/xaml";
var styles = XDocument.Load(Path.Combine(app, "Resources/Styles/FitnessIcons.xaml")).Root!.Elements()
    .ToDictionary(e => (string)e.Attribute(x + "Key")!, e => (string)e.Attribute("TargetType")!);
Require(File.ReadAllText(Path.Combine(app, "App.xaml")).Contains("Resources/Styles/FitnessIcons.xaml"),
    "Словарь иконок не подключён к приложению");
var count = 0;
foreach (var path in Directory.EnumerateFiles(Path.Combine(app, "Pages"), "*.xaml", SearchOption.AllDirectories))
{
    foreach (var element in XDocument.Load(path).Descendants())
    {
        var style = (string?)element.Attribute("Style") ?? "";
        var match = Regex.Match(style, @"^\{StaticResource (Fitness\w+|Icon\w+)\}$");
        if (match.Success)
        {
            var key = match.Groups[1].Value;
            Require(styles.TryGetValue(key, out var target), $"{Path.GetFileName(path)}: отсутствует ресурс {key}");
            Require(target == element.Name.LocalName, $"{Path.GetFileName(path)}: стиль {key} не соответствует типу элемента");
            count++;
        }
        var text = (string?)element.Attribute("Text") ?? "";
        if (text == "{Binding Emoji}")
            Require(style == "{StaticResource FitnessIcon}", $"{Path.GetFileName(path)}: глиф без иконочного шрифта");
        foreach (var rune in text.EnumerateRunes())
            Require(!(rune.Value is >= 0x1F000 and <= 0x1FAFF or 0x2B50),
                $"{Path.GetFileName(path)}: осталась декоративная emoji-иконка");
    }
}
Console.WriteLine($"Проверено: {aliases.Count - 1} глифов в TTF, {count} применений стилей; XAML разобран успешно.");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static ushort U16(byte[] bytes, int offset) => BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(offset, 2));
static uint U32(byte[] bytes, int offset) => BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(offset, 4));

static bool HasGlyph(byte[] font, uint codepoint)
{
    // Unicode cmap форматов 4 и 12. Нулевой glyph id означает отсутствующий символ.
    var cmap = -1;
    for (var i = 0; i < U16(font, 4); i++)
    {
        var record = 12 + 16 * i;
        if (System.Text.Encoding.ASCII.GetString(font, record, 4) == "cmap")
        {
            cmap = (int)U32(font, record + 8);
            break;
        }
    }
    Require(cmap >= 0, "В шрифте нет таблицы cmap");
    for (var i = 0; i < U16(font, cmap + 2); i++)
    {
        var record = cmap + 4 + 8 * i;
        var platform = U16(font, record);
        var encoding = U16(font, record + 2);
        if (platform != 0 && !(platform == 3 && encoding is 1 or 10)) continue;
        var table = cmap + (int)U32(font, record + 4);
        var format = U16(font, table);
        if (format == 12)
        {
            for (var j = 0; j < U32(font, table + 12); j++)
            {
                var group = table + 16 + 12 * j;
                var start = U32(font, group);
                var end = U32(font, group + 4);
                var glyph = U32(font, group + 8);
                if (codepoint >= start && codepoint <= end && glyph + codepoint - start != 0) return true;
            }
        }
        else if (format == 4 && codepoint <= 0xFFFF)
        {
            var segments = U16(font, table + 6) / 2;
            var ends = table + 14;
            var starts = ends + 2 * segments + 2;
            var deltas = starts + 2 * segments;
            var ranges = deltas + 2 * segments;
            for (var j = 0; j < segments; j++)
            {
                var start = U16(font, starts + 2 * j);
                var end = U16(font, ends + 2 * j);
                if (codepoint < start || codepoint > end) continue;
                var delta = U16(font, deltas + 2 * j);
                var distance = U16(font, ranges + 2 * j);
                var glyph = distance == 0 ? (codepoint + delta) & 0xFFFF
                    : U16(font, ranges + 2 * j + distance + 2 * ((int)codepoint - start));
                if (distance != 0 && glyph != 0) glyph = (glyph + delta) & 0xFFFF;
                if (glyph != 0) return true;
            }
        }
    }
    return false;
}
