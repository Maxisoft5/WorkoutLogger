using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.PageModels
{
    /// <summary>
    /// Экран «Сообщения» (M6): список диалогов тренер↔ученик
    /// с превью последнего сообщения и счётчиком непрочитанного.
    /// </summary>
    public partial class ChatListPageModel : ObservableObject
    {
        private readonly IChatApi _api;

        [ObservableProperty]
        private ObservableCollection<ConversationItem> conversations = [];

        [ObservableProperty]
        private bool isEmpty;

        public ChatListPageModel(IChatApi api)
        {
            _api = api;
        }

        public async Task LoadAsync()
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            var currentUserId = (await CurrentUserStore.GetCurrentUser())?.Id ?? "";

            try
            {
                var resp = await _api.GetConversationsAsync($"Bearer {token}");
                Conversations = resp.IsSuccessStatusCode && resp.Content is not null
                    ? new ObservableCollection<ConversationItem>(
                        resp.Content.Select(c => ConversationItem.FromDto(c, currentUserId)))
                    : [];
            }
            catch
            {
                Conversations = [];
            }
            finally
            {
                IsEmpty = Conversations.Count == 0;
            }
        }
    }

    /// <summary>Строка списка диалогов: собеседник, превью, время, badge непрочитанного.</summary>
    public class ConversationItem
    {
        public Guid Id { get; set; }
        public string OtherUserId { get; set; } = "";
        public string? LastMessageText { get; set; }
        public DateTime? LastMessageAtUtc { get; set; }
        public int UnreadCount { get; set; }
        public bool IsTrainerSide { get; set; }

        public string TitleLabel => IsTrainerSide
            ? Loc.Get("Chat_WithStudent")
            : Loc.Get("Chat_WithTrainer");

        public string Emoji => IsTrainerSide ? "🎓" : "🏋️";

        public string PreviewLabel => string.IsNullOrWhiteSpace(LastMessageText)
            ? Loc.Get("Chat_NoMessages")
            : LastMessageText!;

        public string TimeLabel => LastMessageAtUtc is null
            ? ""
            : FormatTime(LastMessageAtUtc.Value.ToLocalTime());

        public bool HasUnread => UnreadCount > 0;
        public string UnreadLabel => UnreadCount > 99 ? "99+" : UnreadCount.ToString();

        private static string FormatTime(DateTime local)
        {
            var culture = new CultureInfo(Loc.Get("_Culture"));
            return local.Date == DateTime.Today
                ? local.ToString("HH:mm", culture)
                : local.ToString("d MMM", culture);
        }

        public static ConversationItem FromDto(ConversationDto dto, string currentUserId)
        {
            var isTrainerSide = dto.TrainerUserId == currentUserId;
            return new ConversationItem
            {
                Id = dto.Id,
                OtherUserId = isTrainerSide ? dto.StudentUserId : dto.TrainerUserId,
                LastMessageText = dto.LastMessageText,
                LastMessageAtUtc = dto.LastMessageAtUtc,
                UnreadCount = dto.UnreadCount,
                IsTrainerSide = isTrainerSide,
            };
        }
    }
}
