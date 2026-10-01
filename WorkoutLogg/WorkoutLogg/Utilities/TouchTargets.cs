namespace WorkoutLogg.Utilities;

public static class TouchTargets
{
    // Для chips с единственной подписью: вся закруглённая поверхность нажимается,
    // а ссылки на Label и CommandParameter остаются прежними.
    public static void ExpandLabel(Border border)
    {
        if (border.Content is not Label label)
            return;

        foreach (var gesture in label.GestureRecognizers.ToArray())
        {
            label.GestureRecognizers.Remove(gesture);
            border.GestureRecognizers.Add(gesture);
        }
        if (border.GestureRecognizers.Count == 0)
            return;

        border.MinimumHeightRequest = Math.Max(48, border.MinimumHeightRequest);
        border.MinimumWidthRequest = Math.Max(48, border.MinimumWidthRequest);
        label.InputTransparent = true;
        label.VerticalOptions = LayoutOptions.Center;
        SemanticProperties.SetDescription(border, label.Text);
    }
}
