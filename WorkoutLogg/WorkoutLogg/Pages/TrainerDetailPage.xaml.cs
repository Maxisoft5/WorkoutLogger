using Modules.Users.Infrastructure.Api;
using System.Globalization;
using WorkoutLogg.Localization;
using WorkoutLogg.PageModels;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public partial class TrainerDetailPage : ContentPage
{
    private readonly TrainersPageModel _trainersModel;
    private readonly ITrainersApi _api;
    private readonly IScheduleApi _scheduleApi;
    private readonly IReviewsApi _reviewsApi;

    private StudentLevel _level = StudentLevel.Beginner;

    private static readonly Color Purple = Color.FromArgb("#7C3AED");
    private static readonly Color White = Colors.White;
    private static readonly Color ChipStroke = Color.FromArgb("#E5E7EB");
    private static readonly Color ChipText = Color.FromArgb("#6B7280");

    public TrainerDetailPage(
        TrainersPageModel trainersModel, ITrainersApi api,
        IScheduleApi scheduleApi, IReviewsApi reviewsApi)
    {
        InitializeComponent();
        _trainersModel = trainersModel;
        _api = api;
        _scheduleApi = scheduleApi;
        _reviewsApi = reviewsApi;
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

        await Task.WhenAll(LoadSlotsAsync(trainer.UserId), LoadReviewsAsync(trainer.UserId));
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

    // ── Chat (M6) ────────────────────────────────────────────────

    private async void OnMessageTapped(object sender, TappedEventArgs e)
    {
        var trainer = _trainersModel.SelectedTrainer;
        if (trainer is null) return;

        await Shell.Current.GoToAsync($"ChatThread?otherUserId={trainer.UserId}");
    }

    // ── Slots & booking (M7) ─────────────────────────────────────

    private async Task LoadSlotsAsync(string trainerUserId)
    {
        var token = await LoginService.GetActiveToken();
        if (string.IsNullOrEmpty(token)) return;

        try
        {
            var resp = await _scheduleApi.GetAvailableSlotsAsync(
                $"Bearer {token}", trainerUserId,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(14));

            SlotsPanel.Children.Clear();
            List<SlotDto> slots = resp.IsSuccessStatusCode && resp.Content is not null
                ? resp.Content.Take(6).ToList()
                : [];

            foreach (var slot in slots)
                SlotsPanel.Children.Add(CreateSlotRow(slot));

            SlotsEmptyLabel.IsVisible = slots.Count == 0;
        }
        catch
        {
            SlotsEmptyLabel.IsVisible = true;
        }
    }

    private View CreateSlotRow(SlotDto slot)
    {
        var culture = new CultureInfo(Loc.Get("_Culture"));
        var start = slot.StartUtc.ToLocalTime();

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
            },
            ColumnSpacing = 10,
        };

        var info = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
        info.Children.Add(new Label
        {
            Text = start.ToString("d MMM yyyy, HH:mm", culture),
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            TextColor = Color.FromArgb("#111827"),
        });
        info.Children.Add(new Label
        {
            Text = $"{slot.DurationMinutes} {Loc.Get("Trainers_Slots_Minutes")}" +
                   (string.IsNullOrWhiteSpace(slot.Note) ? "" : $" · {slot.Note}"),
            FontSize = 12,
            TextColor = Color.FromArgb("#9CA3AF"),
        });
        grid.Add(info, 0);

        var bookLabel = new Label
        {
            Text = Loc.Get("Trainers_Slots_Book"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 13,
            TextColor = Colors.White,
        };
        var bookButton = new Border
        {
            Content = bookLabel,
            BackgroundColor = Purple,
            Padding = new Thickness(16, 8),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(12) },
            VerticalOptions = LayoutOptions.Center,
        };
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await BookSlotAsync(slot);
        bookButton.GestureRecognizers.Add(tap);
        grid.Add(bookButton, 1);

        return new Border
        {
            Content = grid,
            BackgroundColor = Color.FromArgb("#F9FAFB"),
            Padding = new Thickness(14, 10),
            Stroke = new SolidColorBrush(Color.FromArgb("#E5E7EB")),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(12) },
        };
    }

    private async Task BookSlotAsync(SlotDto slot)
    {
        var trainer = _trainersModel.SelectedTrainer;
        if (trainer is null) return;

        var confirmed = await DisplayAlertAsync(
            Loc.Get("Trainers_Slots_BookConfirmTitle"),
            string.Format(Loc.Get("Trainers_Slots_BookConfirmMsg"),
                slot.StartUtc.ToLocalTime().ToString("d MMM yyyy, HH:mm", new CultureInfo(Loc.Get("_Culture")))),
            Loc.Get("Common_Yes"),
            Loc.Get("Common_No"));
        if (!confirmed) return;

        var token = await LoginService.GetActiveToken();
        if (string.IsNullOrEmpty(token)) return;

        PageLoading.Show();
        try
        {
            var resp = await _scheduleApi.BookAsync(
                $"Bearer {token}", new CreateBookingRequestDto(slot.Id, null));

            if (resp.IsSuccessStatusCode)
            {
                await DisplayAlertAsync(
                    Loc.Get("Trainers_Slots_BookedTitle"),
                    Loc.Get("Trainers_Slots_BookedMsg"),
                    Loc.Get("Common_OK"));
                await LoadSlotsAsync(trainer.UserId);
            }
            else
            {
                await DisplayAlertAsync(
                    Loc.Get("Common_Error"),
                    ApiProblem.GetDetail(resp, Loc.Get("Trainers_Slots_BookError")),
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

    // ── Reviews (M8) ─────────────────────────────────────────────

    private async Task LoadReviewsAsync(string trainerUserId)
    {
        var token = await LoginService.GetActiveToken();
        if (string.IsNullOrEmpty(token)) return;

        try
        {
            var resp = await _reviewsApi.GetTrainerReviewsAsync(
                $"Bearer {token}", trainerUserId, page: 1, pageSize: 3);

            ReviewsPanel.Children.Clear();
            List<ReviewDto> reviews = resp.IsSuccessStatusCode && resp.Content is not null
                ? resp.Content.Items
                : [];

            foreach (var review in reviews)
                ReviewsPanel.Children.Add(CreateReviewRow(review));

            ReviewsEmptyLabel.IsVisible = reviews.Count == 0;
        }
        catch
        {
            ReviewsEmptyLabel.IsVisible = true;
        }
    }

    private static View CreateReviewRow(ReviewDto review)
    {
        var culture = new CultureInfo(Loc.Get("_Culture"));
        var stack = new VerticalStackLayout { Spacing = 4 };

        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
            },
        };
        header.Add(new Label
        {
            Text = string.Concat(Enumerable.Repeat("⭐", Math.Clamp(review.Rating, 1, 5))),
            FontSize = 12,
        }, 0);
        header.Add(new Label
        {
            Text = review.CreatedAtUtc.ToLocalTime().ToString("d MMM yyyy", culture),
            FontSize = 11,
            TextColor = Color.FromArgb("#9CA3AF"),
            VerticalOptions = LayoutOptions.Center,
        }, 1);
        stack.Children.Add(header);

        if (!string.IsNullOrWhiteSpace(review.Text))
        {
            stack.Children.Add(new Label
            {
                Text = review.Text,
                FontSize = 13,
                TextColor = Color.FromArgb("#374151"),
            });
        }

        if (!string.IsNullOrWhiteSpace(review.TrainerReply))
        {
            stack.Children.Add(new Label
            {
                Text = $"↳ {review.TrainerReply}",
                FontSize = 12,
                TextColor = Color.FromArgb("#6B7280"),
                Margin = new Thickness(10, 0, 0, 0),
            });
        }

        return new Border
        {
            Content = stack,
            BackgroundColor = Color.FromArgb("#F9FAFB"),
            Padding = new Thickness(14, 10),
            Stroke = new SolidColorBrush(Color.FromArgb("#E5E7EB")),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(12) },
        };
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
