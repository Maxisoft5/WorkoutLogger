using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class TrainingPaymentService(TrainersDbContext dbContext, IWalletService walletService) : ITrainingPaymentService
    {
        public async Task<Result<TrainingPaymentDto>> PayAsync(
            string studentUserId, string trainerUserId, CancellationToken ct = default)
        {
            if (studentUserId == trainerUserId)
                return new Result<TrainingPaymentDto>(TrainerErrors.CannotRequestSelf());

            var trainer = await dbContext.TrainerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == trainerUserId && p.IsActive, ct);
            if (trainer is null)
                return new Result<TrainingPaymentDto>(TrainerErrors.TrainerNotFoundOrInactive());

            // Платить можно только тренеру, который принял заявку ученика.
            var hasAcceptedRequest = await dbContext.TrainingRequests.AnyAsync(r =>
                r.StudentUserId == studentUserId
                && r.TrainerUserId == trainerUserId
                && r.Status == TrainingRequestStatus.Accepted, ct);
            if (!hasAcceptedRequest)
                return new Result<TrainingPaymentDto>(TrainerErrors.NoAcceptedRequest());

            var payment = new TrainingPayment
            {
                Id = Guid.NewGuid(),
                StudentUserId = studentUserId,
                TrainerUserId = trainerUserId,
                PriceFc = trainer.PricePerSession,
                CommissionFc = PlatformFees.CommissionFor(trainer.PricePerSession),
                Status = TrainingPaymentStatus.Held,
                CreatedAtUtc = DateTime.UtcNow
            };
            payment.PayoutFc = payment.PriceFc - payment.CommissionFc;

            // Сначала списание (идемпотентно по Id платежа), затем фиксация платежа.
            var debit = await walletService.DebitAsync(studentUserId, payment.PriceFc,
                WalletTransactionType.TrainingPayment, $"Оплата тренировки, тренер {trainerUserId}",
                $"pay:{payment.Id}", ct);
            if (debit.IsError)
                return new Result<TrainingPaymentDto>(debit.Errors![0]);

            dbContext.TrainingPayments.Add(payment);
            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainingPaymentDto>(payment.MapPayment());
        }

        public async Task<Result<TrainingPaymentDto>> CompleteAsync(
            string studentUserId, Guid paymentId, CancellationToken ct = default)
        {
            var payment = await dbContext.TrainingPayments.FirstOrDefaultAsync(p => p.Id == paymentId, ct);
            if (payment is null)
                return new Result<TrainingPaymentDto>(TrainerErrors.PaymentNotFound());

            if (payment.StudentUserId != studentUserId)
                return new Result<TrainingPaymentDto>(TrainerErrors.NotPaymentStudent());

            if (payment.Status != TrainingPaymentStatus.Held)
                return new Result<TrainingPaymentDto>(TrainerErrors.PaymentNotHeld());

            var payout = await walletService.CreditAsync(payment.TrainerUserId, payment.PayoutFc,
                WalletTransactionType.TrainingPayout,
                $"Выплата за тренировку (комиссия {PlatformFees.CommissionPercent}%: −{payment.CommissionFc} FC)",
                $"payout:{payment.Id}", ct);
            if (payout.IsError)
                return new Result<TrainingPaymentDto>(payout.Errors![0]);

            payment.Status = TrainingPaymentStatus.Completed;
            payment.ResolvedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainingPaymentDto>(payment.MapPayment());
        }

        public async Task<Result<TrainingPaymentDto>> RefundAsync(
            string trainerUserId, Guid paymentId, CancellationToken ct = default)
        {
            var payment = await dbContext.TrainingPayments.FirstOrDefaultAsync(p => p.Id == paymentId, ct);
            if (payment is null)
                return new Result<TrainingPaymentDto>(TrainerErrors.PaymentNotFound());

            if (payment.TrainerUserId != trainerUserId)
                return new Result<TrainingPaymentDto>(TrainerErrors.NotPaymentTrainer());

            if (payment.Status != TrainingPaymentStatus.Held)
                return new Result<TrainingPaymentDto>(TrainerErrors.PaymentNotHeld());

            var refund = await walletService.CreditAsync(payment.StudentUserId, payment.PriceFc,
                WalletTransactionType.Refund, "Возврат оплаты тренировки",
                $"refund:{payment.Id}", ct);
            if (refund.IsError)
                return new Result<TrainingPaymentDto>(refund.Errors![0]);

            payment.Status = TrainingPaymentStatus.Refunded;
            payment.ResolvedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(ct);
            return new Result<TrainingPaymentDto>(payment.MapPayment());
        }

        public async Task<List<TrainingPaymentDto>> GetMyPaymentsAsync(
            string studentUserId, CancellationToken ct = default)
        {
            var items = await dbContext.TrainingPayments
                .AsNoTracking()
                .Where(p => p.StudentUserId == studentUserId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync(ct);
            return items.Select(p => p.MapPayment()).ToList();
        }

        public async Task<List<TrainingPaymentDto>> GetReceivedPaymentsAsync(
            string trainerUserId, CancellationToken ct = default)
        {
            var items = await dbContext.TrainingPayments
                .AsNoTracking()
                .Where(p => p.TrainerUserId == trainerUserId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync(ct);
            return items.Select(p => p.MapPayment()).ToList();
        }
    }
}
