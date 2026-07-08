using Modules.Common.Domain.Results;

namespace Modules.Trainers.Infrastructure.Domain
{
    public static class TrainerErrors
    {
        private const string ErrorPrefix = "Trainers";

        // Пределы цены за тренировку в FitCoins (фильтр на экране поиска — 200–800 FC,
        // но сами карточки допускают более широкий диапазон).
        public const int MinPricePerSession = 100;
        public const int MaxPricePerSession = 50_000;

        public static Error ProfileNotFound() =>
            new($"{ErrorPrefix}.{nameof(ProfileNotFound)}", "Trainer profile not found", ErrorType.NotFound);

        public static Error NoSpecializations() =>
            Error.Validation($"{ErrorPrefix}.{nameof(NoSpecializations)}", "At least one specialization must be selected");

        public static Error NoFormats() =>
            Error.Validation($"{ErrorPrefix}.{nameof(NoFormats)}", "At least one training format must be selected");

        public static Error InvalidPrice() =>
            Error.Validation($"{ErrorPrefix}.{nameof(InvalidPrice)}",
                $"Price per session must be between {MinPricePerSession} and {MaxPricePerSession} FitCoins");
    }
}
