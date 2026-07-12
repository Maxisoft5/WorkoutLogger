using WorkoutLogg.Database;
using WorkoutLogg.Database.Entities;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

public class ChatMessage
{
    public string Role    { get; init; } = "user";
    public string Content { get; init; } = "";
    public bool IsUser      => Role == "user";
    public bool IsAssistant => Role == "assistant";
}

public partial class AiCoachPage : ContentPage
{
    private readonly IAiCoachApi    _api;
    private readonly WorkoutDatabase _db;
    private readonly LanguageService _lang;

    private readonly List<ChatMessage>    _messages = [];
    private AiWorkoutContextDto?          _context;
    private bool                          _isPremium;

    public AiCoachPage(IAiCoachApi api, WorkoutDatabase db, LanguageService lang)
    {
        InitializeComponent();
        _api  = api;
        _db   = db;
        _lang = lang;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    // ── Setup ────────────────────────────────────────────────────

    private async Task LoadAsync()
    {
        var user = await CurrentUserStore.GetCurrentUser();
        _isPremium = user?.IsPremium == true;

        if (!_isPremium)
        {
            ChatScroll.IsVisible   = false;
            TypingIndicator.IsVisible = false;
            PremiumGate.IsVisible  = true;
            KnowsLabel.Text = Loc.Get("AiCoach_PremiumOnly");
            return;
        }

        ChatScroll.IsVisible  = true;
        PremiumGate.IsVisible = false;

        // Build workout context once per page appearance
        _context = await BuildContextAsync();

        var name = user?.FullName?.Split(' ').FirstOrDefault() ?? "there";
        GreetingLabel.Text    = string.Format(Loc.Get("AiCoach_Greeting"), name);
        GreetingSubLabel.Text = Loc.Get("AiCoach_GreetingSub");

        KnowsLabel.Text = _context.TotalSessions > 0
            ? string.Format(Loc.Get("AiCoach_KnowsWorkouts"), _context.TotalSessions)
            : Loc.Get("AiCoach_KnowsZero");

        InsightLabel.Text = BuildInsightText();
    }

    private async Task<AiWorkoutContextDto> BuildContextAsync()
    {
        var stats = await _db.GetProfileStatsAsync();

        // Top PRs as readable string
        var prText = stats.TopPRs.Count > 0
            ? string.Join(", ", stats.TopPRs.Select(pr => $"{pr.ExerciseName}: {pr.MaxWeightKg:F1} kg"))
            : null;

        // Last 5 sessions as brief summary
        var recentLines = new List<string>();
        var today = DateTime.Today;
        for (int i = 0; i < 5; i++)
        {
            var sessions = await _db.GetLogSessionsForDateAsync(today.AddDays(-i));
            foreach (var s in sessions)
            {
                if (s.Exercises.Count == 0) continue;
                var exercises = s.Exercises.Select(e =>
                {
                    var best = e.Sets.Where(set => !set.IsWarmup && set.WeightKg > 0)
                                     .OrderByDescending(set => set.WeightKg * set.Reps)
                                     .FirstOrDefault();
                    return best is not null
                        ? $"{e.ExerciseName} {best.Reps}x{best.WeightKg:F0}kg"
                        : e.ExerciseName;
                });
                recentLines.Add($"{s.Date:dd MMM}: {string.Join(", ", exercises)}");
            }
        }

        return new AiWorkoutContextDto(
            TotalSessions: stats.TotalSessions,
            CurrentStreak: stats.CurrentStreak,
            PersonalRecords: prText,
            RecentSummary: recentLines.Count > 0 ? string.Join("\n", recentLines) : null);
    }

    private string BuildInsightText()
    {
        if (_context is null || _context.TotalSessions == 0)
            return Loc.Get("AiCoach_InsightNoData");

        if (_context.CurrentStreak >= 3)
            return $"🔥 {_context.CurrentStreak}-day streak! Keep the momentum going.";

        if (!string.IsNullOrEmpty(_context.PersonalRecords))
        {
            var first = _context.PersonalRecords!.Split(',')[0].Trim();
            return $"Your top lift: {first}. Keep pushing!";
        }

        return $"You've logged {_context.TotalSessions} sessions total. Great consistency!";
    }

    // ── Quick actions ────────────────────────────────────────────

    private void OnTipTapped(object sender, TappedEventArgs e)        => SendMessage(Loc.Get("AiCoach_Action_Tip_Msg"));
    private void OnAnalyticsTapped(object sender, TappedEventArgs e)  => SendMessage(Loc.Get("AiCoach_Action_Analytics_Msg"));

    // PR-прогноз и генератор плана используют выделенные эндпоинты
    // (/api/ai/forecast, /api/ai/plan) со структурированными промптами на бэкенде.
    private void OnPRForecastTapped(object sender, TappedEventArgs e) =>
        RunSpecialAction(Loc.Get("AiCoach_Action_PR_Msg"), (token, locale, ct) =>
            _api.ForecastAsync(token, new AiForecastRequestDto(_context, locale), ct));

    private void OnPlanTapped(object sender, TappedEventArgs e) =>
        RunSpecialAction(Loc.Get("AiCoach_Action_Plan_Msg"), (token, locale, ct) =>
            _api.GeneratePlanAsync(token, new AiPlanRequestDto(_context, locale), ct));

    private void RunSpecialAction(
        string userText,
        Func<string, string, CancellationToken, Task<Refit.IApiResponse<AiChatResponseDto>>> call)
    {
        if (!_isPremium) return;

        WelcomeSection.IsVisible = false;
        MessagesPanel.IsVisible  = true;
        AddBubble("user", userText);

        _ = FetchSpecialReplyAsync(call);
    }

    private async Task FetchSpecialReplyAsync(
        Func<string, string, CancellationToken, Task<Refit.IApiResponse<AiChatResponseDto>>> call)
    {
        TypingIndicator.IsVisible = true;
        ScrollToBottom();

        try
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token))
            {
                TypingIndicator.IsVisible = false;
                ShowError();
                return;
            }

            var resp = await call($"Bearer {token}", _lang.CurrentCode, CancellationToken.None);

            TypingIndicator.IsVisible = false;

            if (resp.IsSuccessStatusCode && resp.Content?.Success == true)
                AddBubble("assistant", resp.Content.Content);
            else
                ShowError();
        }
        catch
        {
            TypingIndicator.IsVisible = false;
            ShowError();
        }

        ScrollToBottom();
    }

    private void OnSendTapped(object sender, TappedEventArgs e)
    {
        var text = MessageEntry.Text?.Trim();
        if (!string.IsNullOrEmpty(text))
            SendMessage(text);
    }

    // ── Core chat logic ──────────────────────────────────────────

    private void SendMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        MessageEntry.Text = string.Empty;

        // Switch from welcome to chat view
        WelcomeSection.IsVisible = false;
        MessagesPanel.IsVisible  = true;

        AddBubble("user", text);
        _ = FetchReplyAsync(text);
    }

    private async Task FetchReplyAsync(string userText)
    {
        TypingIndicator.IsVisible = true;
        ScrollToBottom();

        try
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token))
            {
                TypingIndicator.IsVisible = false;
                ShowError();
                return;
            }

            var locale = _lang.CurrentCode;
            var request = new AiChatRequestDto(
                Messages: _messages.Select(m => new AiChatMessageDto(m.Role, m.Content)).ToList(),
                Context:  _context,
                Language: locale);

            var resp = await _api.ChatAsync($"Bearer {token}", request);

            TypingIndicator.IsVisible = false;

            if (resp.IsSuccessStatusCode && resp.Content?.Success == true)
                AddBubble("assistant", resp.Content.Content);
            else
                ShowError();
        }
        catch
        {
            TypingIndicator.IsVisible = false;
            ShowError();
        }

        ScrollToBottom();
    }

    private void AddBubble(string role, string content)
    {
        _messages.Add(new ChatMessage { Role = role, Content = content });
        var bubble = CreateBubble(role, content);
        MessagesPanel.Children.Add(bubble);
    }

    private static View CreateBubble(string role, string content)
    {
        var isUser = role == "user";

        var label = new Label
        {
            Text           = content,
            FontSize       = 14,
            TextColor      = isUser ? Colors.White : Color.FromArgb("#111827"),
            LineBreakMode  = LineBreakMode.WordWrap,
        };

        var border = new Border
        {
            Content          = label,
            BackgroundColor  = isUser ? Color.FromArgb("#7C3AED") : Colors.White,
            Padding          = new Thickness(14, 10),
            StrokeThickness  = isUser ? 0 : 1,
            Stroke           = isUser ? null : new SolidColorBrush(Color.FromArgb("#F3F4F6")),
            StrokeShape      = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = isUser
                    ? new CornerRadius(18, 18, 4, 18)
                    : new CornerRadius(18, 18, 18, 4),
            },
            HorizontalOptions = isUser ? LayoutOptions.End : LayoutOptions.Start,
            MaximumWidthRequest = 300,
        };

        return border;
    }

    private void ShowError()
    {
        AddBubble("assistant", Loc.Get("AiCoach_Error"));
    }

    private void ScrollToBottom()
    {
        Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(80);
            ChatScroll.ScrollToAsync(0, double.MaxValue, animated: false);
        });
    }

    private async void OnUpgradeTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("Premium");
}
