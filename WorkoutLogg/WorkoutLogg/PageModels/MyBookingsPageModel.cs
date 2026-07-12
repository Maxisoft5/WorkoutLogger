using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using WorkoutLogg.Localization;
using WorkoutLogg.Services;

namespace WorkoutLogg.PageModels
{
    /// <summary>
    /// Экран «Мои бронирования» (M7, сторона ученика): список бронирований
    /// со статусами и отменой предстоящих тренировок.
    /// </summary>
    public partial class MyBookingsPageModel : ObservableObject
    {
        private readonly IScheduleApi _api;

        [ObservableProperty]
        private ObservableCollection<BookingItem> bookings = [];

        [ObservableProperty]
        private bool isEmpty;

        public MyBookingsPageModel(IScheduleApi api)
        {
            _api = api;
        }

        public async Task LoadAsync()
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return;

            try
            {
                var resp = await _api.GetMyBookingsAsync($"Bearer {token}");
                Bookings = resp.IsSuccessStatusCode && resp.Content is not null
                    ? new ObservableCollection<BookingItem>(resp.Content.Select(BookingItem.FromDto))
                    : [];
            }
            catch
            {
                Bookings = [];
            }
            finally
            {
                IsEmpty = Bookings.Count == 0;
            }
        }

        /// <summary>Отменить бронирование. Возвращает (успех, сообщение об ошибке).</summary>
        public async Task<(bool Ok, string? Error)> CancelAsync(Guid bookingId)
        {
            var token = await LoginService.GetActiveToken();
            if (string.IsNullOrEmpty(token)) return (false, Loc.Get("Common_TryAgain"));

            try
            {
                var resp = await _api.CancelBookingAsync(
                    $"Bearer {token}", bookingId, new CancelBookingRequestDto(null));

                if (resp.IsSuccessStatusCode)
                {
                    await LoadAsync();
                    return (true, null);
                }

                return (false, Modules.Users.Infrastructure.Api.ApiProblem.GetDetail(
                    resp, Loc.Get("Bookings_CancelError")));
            }
            catch
            {
                return (false, Loc.Get("Common_TryAgain"));
            }
        }
    }

    /// <summary>Строка бронирования: дата/время слота, статус, доступность отмены.</summary>
    public class BookingItem
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "";
        public DateTime? StartUtc { get; set; }
        public DateTime? EndUtc { get; set; }
        public string? Note { get; set; }

        public string StatusLabel => Loc.Get($"Bookings_Status_{Status}");

        public string Emoji => Status switch
        {
            "Pending" => "⏳",
            "Confirmed" => "✅",
            "Completed" => "🏆",
            "Cancelled" => "❌",
            "NoShow" => "🚫",
            _ => "📅",
        };

        public Color StatusColor => Status switch
        {
            "Confirmed" => Color.FromArgb("#16A34A"),
            "Completed" => Color.FromArgb("#7C3AED"),
            "Cancelled" or "NoShow" => Color.FromArgb("#EF4444"),
            _ => Color.FromArgb("#F59E0B"),
        };

        public string WhenLabel
        {
            get
            {
                if (StartUtc is null) return "—";
                var culture = new CultureInfo(Loc.Get("_Culture"));
                var start = StartUtc.Value.ToLocalTime();
                var end = EndUtc?.ToLocalTime();
                return end is null
                    ? start.ToString("d MMM yyyy, HH:mm", culture)
                    : $"{start.ToString("d MMM yyyy, HH:mm", culture)}–{end.Value.ToString("HH:mm", culture)}";
            }
        }

        public string SubLabel => string.IsNullOrWhiteSpace(Note)
            ? WhenLabel
            : $"{WhenLabel} · {Note}";

        /// <summary>Отменить можно только предстоящие Pending/Confirmed-бронирования.</summary>
        public bool CanCancel =>
            Status is "Pending" or "Confirmed"
            && StartUtc is not null
            && StartUtc.Value > DateTime.UtcNow;

        public static BookingItem FromDto(BookingDto dto) => new()
        {
            Id = dto.Id,
            Status = dto.Status,
            StartUtc = dto.Slot?.StartUtc,
            EndUtc = dto.Slot?.EndUtc,
            Note = dto.Slot?.Note,
        };
    }
}
