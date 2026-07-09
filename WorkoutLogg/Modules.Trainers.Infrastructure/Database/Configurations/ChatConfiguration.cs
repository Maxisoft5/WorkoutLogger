using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Database.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.StudentUserId).IsRequired().HasMaxLength(450);
            builder.Property(c => c.TrainerUserId).IsRequired().HasMaxLength(450);
            builder.HasIndex(c => new { c.StudentUserId, c.TrainerUserId }).IsUnique();
            builder.HasIndex(c => c.TrainerUserId);
        }
    }

    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.SenderUserId).IsRequired().HasMaxLength(450);
            builder.Property(m => m.Text).IsRequired().HasMaxLength(2000);
            builder.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(m => new { m.ConversationId, m.SentAtUtc });
        }
    }
}
