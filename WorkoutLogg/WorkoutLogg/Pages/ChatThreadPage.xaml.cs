using System.Globalization;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.Pages;

/// <summary>
/// Диалог тренер↔ученик (M6). MVP работает на поллинге:
/// сообщения перечитываются по таймеру, входящие помечаются прочитанными.
/// Открывается по conversationId (из списка) или otherUserId (кнопка «Написать»).
/// </summary>
[QueryProperty(nameof(ConversationId), "conversationId")]
[QueryProperty(nameof(OtherUserId), "otherUserId")]
public partial class ChatThreadPage : ContentPage
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

    private readonly IChatApi _api;

    private Guid _conversationId;
    private string _currentUserId = "";
    private readonly HashSet<Guid> _renderedMessageIds = [];
    private IDispatcherTimer? _pollTimer;
    private bool _isLoading;

    public string? ConversationId { get; set; }
    public string? OtherUserId { get; set; }

    public ChatThreadPage(IChatApi api)
    {
        InitializeComponent();
        _api = api;
        PageLoading.Preload();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _currentUserId = (await CurrentUserStore.GetCurrentUser())?.Id ?? "";

        PageLoading.Show();
        try
        {
            if (!await EnsureConversationAsync())
            {
                await DisplayAlertAsync(Loc.Get("Common_Error"), Loc.Get("Chat_OpenError"), Loc.Get("Common_OK"));
                await Shell.Current.GoToAsync("..");
                return;
            }

            await RefreshMessagesAsync();
        }
        finally
        {
            PageLoading.Hide();
        }

        StartPolling();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopPolling();
        PageLoading.Preload();
    }

    // ── Setup ────────────────────────────────────────────────────

    private async Task<bool> EnsureConversationAsync()
    {
        if (Guid.TryParse(ConversationId, out var existing))
        {
            _conversationId = existing;
            return true;
        }

        if (string.IsNullOrEmpty(OtherUserId)) return false;

        var token = await LoginService.GetActiveToken();
        if (string.IsNullOrEmpty(token)) return false;

        try
        {
            var resp = await _api.OpenConversationAsync(
                $"Bearer {token}", new OpenConversationRequestDto(OtherUserId));
            if (!resp.IsSuccessStatusCode || resp.Content is null) return false;

            _conversationId = resp.Content.Id;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void StartPolling()
    {
        _pollTimer = Dispatcher.CreateTimer();
        _pollTimer.Interval = PollInterval;
        _pollTimer.Tick += async (_, _) => await RefreshMessagesAsync();
        _pollTimer.Start();
    }

    private void StopPolling()
    {
        _pollTimer?.Stop();
        _pollTimer = null;
    }

    // ── Messages ─────────────────────────────────────────────────

    private async Task RefreshMessagesAsync()
    {
        if (_isLoading || _conversationId == Guid.Empty) return;
        _isLoading = true;

        try
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            var resp = await _api.GetMessagesAsync($"Bearer {token}", _conversationId, page: 1, pageSize: 50);
            if (!resp.IsSuccessStatusCode || resp.Content is null) return;

            var newMessages = resp.Content.Items
                .Where(m => !_renderedMessageIds.Contains(m.Id))
                .ToList();

            if (newMessages.Count > 0)
            {
                foreach (var message in newMessages)
                    AddBubble(message);
                ScrollToBottom();

                if (newMessages.Any(m => m.SenderUserId != _currentUserId))
                    await _api.MarkReadAsync($"Bearer {token}", _conversationId);
            }

            EmptyState.IsVisible = _renderedMessageIds.Count == 0;
        }
        catch
        {
            // поллинг не должен ронять страницу — следующая итерация попробует снова
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OnSendCompleted(object sender, EventArgs e) => _ = SendAsync();

    private void OnSendTapped(object sender, TappedEventArgs e) => _ = SendAsync();

    private async Task SendAsync()
    {
        var text = MessageEntry.Text?.Trim();
        if (string.IsNullOrEmpty(text) || _conversationId == Guid.Empty) return;

        MessageEntry.Text = string.Empty;

        try
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            var resp = await _api.SendMessageAsync(
                $"Bearer {token}", _conversationId, new SendMessageRequestDto(text));

            if (resp.IsSuccessStatusCode && resp.Content is not null)
            {
                AddBubble(resp.Content);
                EmptyState.IsVisible = false;
                ScrollToBottom();
            }
            else
            {
                await DisplayAlertAsync(Loc.Get("Common_Error"), Loc.Get("Chat_SendError"), Loc.Get("Common_OK"));
            }
        }
        catch
        {
            await DisplayAlertAsync(Loc.Get("Common_Error"), Loc.Get("Common_TryAgain"), Loc.Get("Common_OK"));
        }
    }

    // ── Rendering ────────────────────────────────────────────────

    private void AddBubble(ChatMessageDto message)
    {
        if (!_renderedMessageIds.Add(message.Id)) return;

        var isMine = message.SenderUserId == _currentUserId;
        var culture = new CultureInfo(Loc.Get("_Culture"));
        var local = message.SentAtUtc.ToLocalTime();
        var timeText = local.Date == DateTime.Today
            ? local.ToString("HH:mm", culture)
            : local.ToString("d MMM HH:mm", culture);

        var stack = new VerticalStackLayout
        {
            Spacing = 2,
            HorizontalOptions = isMine ? LayoutOptions.End : LayoutOptions.Start,
        };

        stack.Children.Add(new Border
        {
            Content = new Label
            {
                Text = message.Text,
                FontSize = 14,
                TextColor = isMine ? Colors.White : Color.FromArgb("#111827"),
                LineBreakMode = LineBreakMode.WordWrap,
            },
            BackgroundColor = isMine ? Color.FromArgb("#7C3AED") : Colors.White,
            Padding = new Thickness(14, 10),
            StrokeThickness = isMine ? 0 : 1,
            Stroke = isMine ? null : new SolidColorBrush(Color.FromArgb("#F3F4F6")),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = isMine
                    ? new CornerRadius(18, 18, 4, 18)
                    : new CornerRadius(18, 18, 18, 4),
            },
            MaximumWidthRequest = 300,
        });

        stack.Children.Add(new Label
        {
            Text = timeText,
            FontSize = 10,
            TextColor = Color.FromArgb("#9CA3AF"),
            HorizontalOptions = isMine ? LayoutOptions.End : LayoutOptions.Start,
        });

        MessagesPanel.Children.Add(stack);
    }

    private void ScrollToBottom()
    {
        Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(80);
            await MessagesScroll.ScrollToAsync(0, double.MaxValue, animated: false);
        });
    }

    private async void OnBackTapped(object sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("..");
}
