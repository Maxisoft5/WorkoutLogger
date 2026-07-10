using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database
{
    public class TrainersDbContext : DbContext
    {
        public DbSet<TrainerProfile> TrainerProfiles { get; set; } = null!;
        public DbSet<TrainingRequest> TrainingRequests { get; set; } = null!;
        public DbSet<Wallet> Wallets { get; set; } = null!;
        public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
        public DbSet<TrainingPayment> TrainingPayments { get; set; } = null!;
        public DbSet<Conversation> Conversations { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<TrainerVerification> TrainerVerifications { get; set; } = null!;
        public DbSet<VerificationDocument> VerificationDocuments { get; set; } = null!;

        public TrainersDbContext(DbContextOptions<TrainersDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("trainers");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainersDbContext).Assembly);
        }
    }
}
