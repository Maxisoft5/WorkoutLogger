using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class TrainingPaymentDto
    {
        public Guid Id { get; set; }
        public string StudentUserId { get; set; } = null!;
        public string TrainerUserId { get; set; } = null!;
        public int PriceFc { get; set; }
        public int CommissionFc { get; set; }
        public int PayoutFc { get; set; }
        public TrainingPaymentStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ResolvedAtUtc { get; set; }
    }

    public static class TrainingPaymentMapper
    {
        public static TrainingPaymentDto MapPayment(this TrainingPayment payment) => new()
        {
            Id = payment.Id,
            StudentUserId = payment.StudentUserId,
            TrainerUserId = payment.TrainerUserId,
            PriceFc = payment.PriceFc,
            CommissionFc = payment.CommissionFc,
            PayoutFc = payment.PayoutFc,
            Status = payment.Status,
            CreatedAtUtc = payment.CreatedAtUtc,
            ResolvedAtUtc = payment.ResolvedAtUtc
        };
    }

    public interface ITrainingPaymentService
    {
        /// <summary>
        /// Ученик оплачивает тренировку: цена берётся с карточки тренера,
        /// FC списываются с кошелька и удерживаются платформой (эскроу).
        /// Требуется принятая заявка между учеником и тренером.
        /// </summary>
        Task<Result<TrainingPaymentDto>> PayAsync(string studentUserId, string trainerUserId, CancellationToken ct = default);

        /// <summary>Ученик подтверждает проведённую тренировку — тренеру выплачивается цена минус комиссия.</summary>
        Task<Result<TrainingPaymentDto>> CompleteAsync(string studentUserId, Guid paymentId, CancellationToken ct = default);

        /// <summary>Тренер возвращает оплату (тренировка не состоится) — FC возвращаются ученику.</summary>
        Task<Result<TrainingPaymentDto>> RefundAsync(string trainerUserId, Guid paymentId, CancellationToken ct = default);

        /// <summary>Платежи текущего ученика (новые сверху).</summary>
        Task<List<TrainingPaymentDto>> GetMyPaymentsAsync(string studentUserId, CancellationToken ct = default);

        /// <summary>Оплаты, полученные тренером (новые сверху).</summary>
        Task<List<TrainingPaymentDto>> GetReceivedPaymentsAsync(string trainerUserId, CancellationToken ct = default);
    }
}
