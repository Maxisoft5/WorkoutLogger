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

        public static Error RequestNotFound() =>
            new($"{ErrorPrefix}.{nameof(RequestNotFound)}", "Training request not found", ErrorType.NotFound);

        public static Error RequestAlreadyPending() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(RequestAlreadyPending)}",
                "There is already a pending request to this trainer");

        public static Error OpenRequestAlreadyPending() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(OpenRequestAlreadyPending)}",
                "There is already a pending open request");

        public static Error RequestNotPending() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(RequestNotPending)}",
                "The request has already been responded to or cancelled");

        public static Error NotRequestOwner() =>
            Error.Forbidden($"{ErrorPrefix}.{nameof(NotRequestOwner)}", "The request belongs to another student");

        public static Error NotRequestTrainer() =>
            Error.Forbidden($"{ErrorPrefix}.{nameof(NotRequestTrainer)}", "The request is addressed to another trainer");

        public static Error CannotRequestSelf() =>
            Error.Validation($"{ErrorPrefix}.{nameof(CannotRequestSelf)}", "You cannot send a training request to yourself");

        public static Error TrainerNotFoundOrInactive() =>
            new($"{ErrorPrefix}.{nameof(TrainerNotFoundOrInactive)}",
                "Trainer profile not found or inactive", ErrorType.NotFound);
    }
}
