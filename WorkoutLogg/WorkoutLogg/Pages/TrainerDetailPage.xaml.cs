using Modules.Users.Infrastructure.Api;
using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public partial class TrainerDetailPage : ContentPage
{
    private readonly TrainersPageModel _trainersModel;
    private readonly ITrainersApi _api;

    private StudentLevel _level = StudentLevel.Beginner;

    private static readonly Color Purple = Color.FromArgb("#7C3AED");
    private static readonly Color White = Colors.White;
    private static readonly Color ChipStroke = Color.FromArgb("#E5E7EB");
    private static readonly Color ChipText = Color.FromArgb("#6B7280");

    public TrainerDetailPage(TrainersPageModel trainersModel, ITrainersApi api)
    {
        InitializeComponent();
        _trainersModel = trainersModel;
        _api = api;
        PageLoading.Preload();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var trainer = _trainersModel.SelectedTrainer;
        if (trainer is null)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        BindingContext = trainer;
        UpdateLevelChips();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        PageLoading.Preload();
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private void OnLevelTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not string name) return;
        if (!Enum.TryParse<StudentLevel>(name, out var level)) return;

        _level = level;
        UpdateLevelChips();
    }

    private void UpdateLevelChips()
    {
        SetChip(LevelBeginner, LevelBeginnerLabel, _level == StudentLevel.Beginner);
        SetChip(LevelIntermediate, LevelIntermediateLabel, _level == StudentLevel.Intermediate);
        SetChip(LevelAdvanced, LevelAdvancedLabel, _level == StudentLevel.Advanced);
    }

    private static void SetChip(Border border, Label label, bool active)
    {
        border.BackgroundColor = active ? Purple : White;
        border.Stroke = active ? Purple : ChipStroke;
        border.StrokeThickness = active ? 0 : 1.5;
        label.TextColor = active ? White : ChipText;
    }

    private async void OnSubmitTapped(object sender, TappedEventArgs e)
    {
        var trainer = _trainersModel.SelectedTrainer;
        if (trainer is null) return;

        var token = await LoginService.GetActiveToken();
        if (string.IsNullOrEmpty(token))
        {
            await DisplayAlertAsync(Loc.Get("Common_Error"), Loc.Get("Common_TryAgain"), Loc.Get("Common_OK"));
            return;
        }

        int? budget = int.TryParse(BudgetEntry.Text, out var parsed) && parsed > 0 ? parsed : null;

        var goal = trainer.Specializations == TrainerSpecializations.None
            ? TrainerSpecializations.Strength
            : trainer.Specializations;

        var body = new CreateTrainingRequestDto(
            TrainerUserId: trainer.UserId,
            Goal: goal,
            Level: _level,
            Formats: trainer.Formats,
            Schedule: null,
            Budget: budget,
            Message: string.IsNullOrWhiteSpace(MessageEditor.Text) ? null : MessageEditor.Text.Trim());

        PageLoading.Show();
        try
        {
            var resp = await _api.CreateRequestAsync($"Bearer {token}", body);
            if (resp.IsSuccessStatusCode)
            {
                await DisplayAlertAsync(
                    Loc.Get("Trainers_Request_SentTitle"),
                    Loc.Get("Trainers_Request_SentMsg"),
                    Loc.Get("Common_OK"));
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlertAsync(
                    Loc.Get("Common_Error"),
                    ApiProblem.GetDetail(resp, Loc.Get("Trainers_Request_Error")),
                    Loc.Get("Common_OK"));
            }
        }
        catch
        {
            await DisplayAlertAsync(Loc.Get("Common_Error"), Loc.Get("Common_TryAgain"), Loc.Get("Common_OK"));
        }
        finally
        {
            PageLoading.Hide();
        }
    }
}
