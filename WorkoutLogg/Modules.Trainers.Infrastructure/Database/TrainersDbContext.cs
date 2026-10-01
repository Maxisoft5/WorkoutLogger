using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Common.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database
{
    public class TrainersDbContext : DbContext, ITenantDbContext
    {
        public DbSet<TrainerProfile> TrainerProfiles { get; set; } = null!;
        public DbSet<TrainingRequest> TrainingRequests { get; set; } = null!;
        public DbSet<Wallet> Wallets { get; set; } = null!;
        public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
        public DbSet<TrainingPayment> TrainingPayments { get; set; } = null!;
        public DbSet<Conversation> Conversations { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<TrainerVerification> TrainerVerifications { get; set; } = null!;
        public DbSet<VerificationDocument> VerificationDocuments { get; set; } = null!;
        public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        public TrainersDbContext(DbContextOptions<TrainersDbContext> options, TenantContext? tenant = null) : base(options) { Schema = tenant?.Current.Schema ?? "trainers"; }

        public string Schema { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainersDbContext).Assembly);
        }
    }
}
