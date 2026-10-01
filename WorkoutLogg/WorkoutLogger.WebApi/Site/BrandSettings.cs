using System.ComponentModel.DataAnnotations;

namespace WorkoutLogger.WebApi.Site;

public sealed record ContentBlock
{
    [Required, StringLength(100)] public string Title { get; init; } = "";
    [StringLength(1500)] public string Text { get; init; } = "";
    public bool Visible { get; init; } = true;
}

public sealed record BrandSettings
{
    [Required, StringLength(80)] public string Name { get; init; } = "WorkoutLogg";
    [Required, StringLength(160)] public string Tagline { get; init; } = "Твоя следующая сильная версия";
    [Required, RegularExpression("^#[0-9a-fA-F]{6}$")] public string Accent { get; init; } = "#f97316";
    [Required, RegularExpression("^(light|dark|system)$")] public string DefaultTheme { get; init; } = "dark";
    [Required, RegularExpression("^(rounded|square)$")] public string Shape { get; init; } = "rounded";
    [Required, MaxLength(12)] public List<ContentBlock> HomeBlocks { get; init; } = [];
    [Required, MaxLength(12)] public List<ContentBlock> ProfileBlocks { get; init; } = [];
}

public sealed record SaveBrandRequest([Required] BrandSettings Settings, long Version);
public sealed record PublishBrandRequest(long Version);
