using System.Numerics;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Users.DTO.Users;

namespace Modules.Trainers.Infrastructure.Services
{
    /// <summary>
    /// Детерминированный match-скор v1 (проценты на карточках экрана 02).
    /// Веса: специализации — 50, формат — 30, цена — 20.
    /// Отсутствующее предпочтение даёт полный балл компонента (не штрафуем тренера
    /// за то, что ученик ничего не указал). ML-персонализация — отдельный этап.
    /// </summary>
    public static class TrainerMatchCalculator
    {
        public const int SpecializationsWeight = 50;
        public const int FormatWeight = 30;
        public const int PriceWeight = 20;

        public static int CalculateMatch(TrainerProfile profile, StudentPreferences preferences)
        {
            return SpecializationsScore(profile.Specializations, preferences.DesiredSpecializations)
                 + FormatScore(profile.Formats, preferences.DesiredFormats)
                 + PriceScore(profile.PricePerSession, preferences.Budget);
        }

        private static int SpecializationsScore(TrainerSpecializations trainer, TrainerSpecializations desired)
        {
            if (desired == TrainerSpecializations.None)
                return SpecializationsWeight;

            var desiredCount = BitOperations.PopCount((uint)desired);
            var matchedCount = BitOperations.PopCount((uint)(trainer & desired));
            return (int)Math.Round(SpecializationsWeight * (double)matchedCount / desiredCount);
        }

        private static int FormatScore(TrainingFormats trainer, TrainingFormats desired)
        {
            if (desired == TrainingFormats.None)
                return FormatWeight;

            return (trainer & desired) != TrainingFormats.None ? FormatWeight : 0;
        }

        private static int PriceScore(int price, int? budget)
        {
            if (budget is null or <= 0)
                return PriceWeight;

            if (price <= budget)
                return PriceWeight;

            // До +20% сверх бюджета — половина балла, дальше 0.
            return price <= budget * 1.2 ? PriceWeight / 2 : 0;
        }

        /// <summary>
        /// Маппинг целей ученика (онбординг Users) в специализации тренера —
        /// источник блока «Подобрано для вас».
        /// </summary>
        public static TrainerSpecializations MapGoalsToSpecializations(IEnumerable<UserGoalVariant>? goals)
        {
            var result = TrainerSpecializations.None;
            if (goals is null) return result;

            foreach (var goal in goals)
            {
                result |= goal switch
                {
                    UserGoalVariant.LoseFat => TrainerSpecializations.WeightLoss,
                    UserGoalVariant.BuildMuscle => TrainerSpecializations.Strength,
                    UserGoalVariant.IncreaseStrength => TrainerSpecializations.Strength,
                    UserGoalVariant.ImporveEndurance => TrainerSpecializations.Running | TrainerSpecializations.Crossfit,
                    UserGoalVariant.Flexibility => TrainerSpecializations.Yoga,
                    UserGoalVariant.StayActive => TrainerSpecializations.None, // подходит любой тренер
                    _ => TrainerSpecializations.None
                };
            }

            return result;
        }
    }
}
