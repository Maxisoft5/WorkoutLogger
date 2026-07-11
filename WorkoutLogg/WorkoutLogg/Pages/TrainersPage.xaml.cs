using WorkoutLogg.PageModels;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public partial class TrainersPage : ContentPage
{
    private readonly TrainersPageModel _vm;

    private static readonly Color Purple = Color.FromArgb("#7C3AED");
    private static readonly Color White = Colors.White;
    private static readonly Color ChipStroke = Color.FromArgb("#E5E7EB");
    private static readonly Color ChipText = Color.FromArgb("#6B7280");

    public TrainersPage(TrainersPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        PageLoading.Preload();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        PageLoading.Show();
        try
        {
            await _vm.LoadAsync();
            UpdateSpecChips();
            UpdateSortChips();
            UpdateRatingChips();
        }
        finally
        {
            PageLoading.Hide();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        PageLoading.Preload();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnSpecTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string name) return;
        if (!Enum.TryParse<TrainerSpecializations>(name, out var flag)) return;

        await _vm.ToggleSpecializationAsync(flag);
        UpdateSpecChips();
    }

    private async void OnSortTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string name) return;
        if (!Enum.TryParse<TrainerSortBy>(name, out var sort)) return;

        await _vm.SetSortAsync(sort);
        UpdateSortChips();
    }

    private async void OnRatingTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string raw) return;
        double? min = double.TryParse(raw, System.Globalization.CultureInfo.InvariantCulture, out var v) && v > 0
            ? v
            : null;

        await _vm.SetMinRatingAsync(min);
        UpdateRatingChips();
    }

    private async void OnTrainerTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string userId) return;
        var item = _vm.FindByUserId(userId);
        if (item is null) return;

        _vm.SelectedTrainer = item;
        await Shell.Current.GoToAsync("TrainerDetail");
    }

    private void UpdateSpecChips()
    {
        var s = _vm.SelectedSpecializations;
        SetChip(ChipStrength, ChipStrengthLabel, s.HasFlag(TrainerSpecializations.Strength));
        SetChip(ChipWeightLoss, ChipWeightLossLabel, s.HasFlag(TrainerSpecializations.WeightLoss));
        SetChip(ChipCrossfit, ChipCrossfitLabel, s.HasFlag(TrainerSpecializations.Crossfit));
        SetChip(ChipYoga, ChipYogaLabel, s.HasFlag(TrainerSpecializations.Yoga));
        SetChip(ChipRehabilitation, ChipRehabilitationLabel, s.HasFlag(TrainerSpecializations.Rehabilitation));
        SetChip(ChipRunning, ChipRunningLabel, s.HasFlag(TrainerSpecializations.Running));
    }

    private void UpdateSortChips()
    {
        var sort = _vm.SortBy;
        SetChip(SortMatch, SortMatchLabel, sort == TrainerSortBy.Match);
        SetChip(SortPriceAsc, SortPriceAscLabel, sort == TrainerSortBy.PriceAsc);
        SetChip(SortPriceDesc, SortPriceDescLabel, sort == TrainerSortBy.PriceDesc);
        SetChip(SortNewest, SortNewestLabel, sort == TrainerSortBy.Newest);
    }

    private void UpdateRatingChips()
    {
        var min = _vm.MinRating;
        SetChip(RatingAny, RatingAnyLabel, min is null);
        SetChip(Rating45, Rating45Label, min is 4.5);
        SetChip(Rating48, Rating48Label, min is 4.8);
    }

    private static void SetChip(Border border, Label label, bool active)
    {
        border.BackgroundColor = active ? Purple : White;
        border.Stroke = active ? Purple : ChipStroke;
        border.StrokeThickness = active ? 0 : 1.5;
        label.TextColor = active ? White : ChipText;
    }
}
