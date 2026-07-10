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

        public static Error InvalidAmount() =>
            Error.Validation($"{ErrorPrefix}.{nameof(InvalidAmount)}", "Amount must be positive");

        public static Error InsufficientFunds() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(InsufficientFunds)}", "Not enough FitCoins in the wallet");

        public static Error DuplicateOperation() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(DuplicateOperation)}", "This operation has already been processed");

        public static Error StreakNotReached() =>
            Error.Validation($"{ErrorPrefix}.{nameof(StreakNotReached)}",
                $"Workout streak of {RewardAmounts.StreakLengthDays} consecutive days is required");

        public static Error StreakBonusAlreadyClaimed() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(StreakBonusAlreadyClaimed)}",
                "Streak bonus has already been claimed for the current series");

        public static Error PaymentNotFound() =>
            new($"{ErrorPrefix}.{nameof(PaymentNotFound)}", "Training payment not found", ErrorType.NotFound);

        public static Error NotPaymentStudent() =>
            Error.Forbidden($"{ErrorPrefix}.{nameof(NotPaymentStudent)}", "The payment belongs to another student");

        public static Error NotPaymentTrainer() =>
            Error.Forbidden($"{ErrorPrefix}.{nameof(NotPaymentTrainer)}", "The payment is addressed to another trainer");

        public static Error PaymentNotHeld() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(PaymentNotHeld)}", "The payment has already been resolved");

        public static Error NoAcceptedRequest() =>
            Error.Validation($"{ErrorPrefix}.{nameof(NoAcceptedRequest)}",
                "You can pay only a trainer who accepted your training request");

        public static Error ConversationNotFound() =>
            new($"{ErrorPrefix}.{nameof(ConversationNotFound)}", "Conversation not found", ErrorType.NotFound);

        public static Error NotConversationParticipant() =>
            Error.Forbidden($"{ErrorPrefix}.{nameof(NotConversationParticipant)}",
                "You are not a participant of this conversation");

        public static Error NoChatRelationship() =>
            Error.Validation($"{ErrorPrefix}.{nameof(NoChatRelationship)}",
                "Chat is available only between a student and a trainer connected by a training request");

        public static Error EmptyMessage() =>
            Error.Validation($"{ErrorPrefix}.{nameof(EmptyMessage)}", "Message text must not be empty");

        // ─── Reviews (M8) ─────────────────────────────────────────────────────

        public static Error ReviewNotFound() =>
            new($"{ErrorPrefix}.{nameof(ReviewNotFound)}", "Review not found", ErrorType.NotFound);

        public static Error InvalidRating() =>
            Error.Validation($"{ErrorPrefix}.{nameof(InvalidRating)}", "Rating must be between 1 and 5");

        public static Error ReviewRequiresCompletedPayment() =>
            Error.Validation($"{ErrorPrefix}.{nameof(ReviewRequiresCompletedPayment)}",
                "A review can only be left after a completed training payment");

        public static Error ReviewAlreadyExists() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(ReviewAlreadyExists)}",
                "A review has already been submitted for this payment");

        public static Error NotReviewOwner() =>
            Error.Forbidden($"{ErrorPrefix}.{nameof(NotReviewOwner)}", "The review belongs to another student");

        public static Error ReviewReplyForbidden() =>
            Error.Forbidden($"{ErrorPrefix}.{nameof(ReviewReplyForbidden)}", "Only the reviewed trainer can reply");

        public static Error ReplyAlreadyExists() =>
            Error.Conflict($"{ErrorPrefix}.{nameof(ReplyAlreadyExists)}", "A reply has already been added to this review");
    }
}
