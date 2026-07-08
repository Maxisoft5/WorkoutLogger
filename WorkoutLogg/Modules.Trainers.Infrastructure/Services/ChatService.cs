using Microsoft.EntityFrameworkCore;
using Modules.Common.Domain.Results;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;

namespace Modules.Trainers.Infrastructure.Services
{
    public class ChatService(TrainersDbContext dbContext) : IChatService
    {
        public async Task<Result<ConversationDto>> GetOrCreateConversationAsync(
            string userId, string otherUserId, CancellationToken ct = default)
        {
            if (userId == otherUserId)
                return new Result<ConversationDto>(TrainerErrors.NoChatRelationship());

            // Заявка между двумя пользователями в любой комбинации ролей.
            var relationship = await dbContext.TrainingRequests
                .AsNoTracking()
                .Where(r => r.Status == TrainingRequestStatus.Pending || r.Status == TrainingRequestStatus.Accepted)
                .Where(r =>
                    (r.StudentUserId == userId && r.TrainerUserId == otherUserId)
                    || (r.StudentUserId == otherUserId && r.TrainerUserId == userId))
                .Select(r => new { r.StudentUserId, r.TrainerUserId })
                .FirstOrDefaultAsync(ct);
            if (relationship is null)
                return new Result<ConversationDto>(TrainerErrors.NoChatRelationship());

            var conversation = await dbContext.Conversations.FirstOrDefaultAsync(c =>
                c.StudentUserId == relationship.StudentUserId
                && c.TrainerUserId == relationship.TrainerUserId, ct);

            if (conversation is null)
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    StudentUserId = relationship.StudentUserId,
                    TrainerUserId = relationship.TrainerUserId!,
                    CreatedAtUtc = DateTime.UtcNow
                };
                dbContext.Conversations.Add(conversation);
                await dbContext.SaveChangesAsync(ct);
            }

            return new Result<ConversationDto>(await BuildConversationDtoAsync(conversation, userId, ct));
        }

        public async Task<List<ConversationDto>> GetConversationsAsync(
            string userId, CancellationToken ct = default)
        {
            var conversations = await dbContext.Conversations
                .AsNoTracking()
                .Where(c => c.StudentUserId == userId || c.TrainerUserId == userId)
                .OrderByDescending(c => c.LastMessageAtUtc ?? c.CreatedAtUtc)
                .ToListAsync(ct);

            var result = new List<ConversationDto>(conversations.Count);
            foreach (var conversation in conversations)
                result.Add(await BuildConversationDtoAsync(conversation, userId, ct));
            return result;
        }

        public async Task<Result<ChatMessagesPageDto>> GetMessagesAsync(
            string userId, Guid conversationId, int page, int pageSize, CancellationToken ct = default)
        {
            var access = await CheckAccessAsync(userId, conversationId, ct);
            if (access is not null)
                return new Result<ChatMessagesPageDto>(access);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = dbContext.ChatMessages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversationId);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(m => m.SentAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            items.Reverse(); // внутри страницы — хронологический порядок

            return new Result<ChatMessagesPageDto>(new ChatMessagesPageDto
            {
                Items = items.Select(m => m.MapMessage()).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            });
        }

        public async Task<Result<ChatMessageDto>> SendMessageAsync(
            string userId, Guid conversationId, string text, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new Result<ChatMessageDto>(TrainerErrors.EmptyMessage());

            var conversation = await dbContext.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId, ct);
            if (conversation is null)
                return new Result<ChatMessageDto>(TrainerErrors.ConversationNotFound());

            if (conversation.StudentUserId != userId && conversation.TrainerUserId != userId)
                return new Result<ChatMessageDto>(TrainerErrors.NotConversationParticipant());

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderUserId = userId,
                Text = text.Trim(),
                SentAtUtc = DateTime.UtcNow
            };

            conversation.LastMessageAtUtc = message.SentAtUtc;
            dbContext.ChatMessages.Add(message);
            await dbContext.SaveChangesAsync(ct);
            return new Result<ChatMessageDto>(message.MapMessage());
        }

        public async Task<Result<int>> MarkReadAsync(
            string userId, Guid conversationId, CancellationToken ct = default)
        {
            var access = await CheckAccessAsync(userId, conversationId, ct);
            if (access is not null)
                return new Result<int>(access);

            var unread = await dbContext.ChatMessages
                .Where(m => m.ConversationId == conversationId
                    && m.SenderUserId != userId
                    && m.ReadAtUtc == null)
                .ToListAsync(ct);

            var now = DateTime.UtcNow;
            foreach (var message in unread)
                message.ReadAtUtc = now;

            await dbContext.SaveChangesAsync(ct);
            return new Result<int>(unread.Count);
        }

        private async Task<Error?> CheckAccessAsync(string userId, Guid conversationId, CancellationToken ct)
        {
            var conversation = await dbContext.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == conversationId, ct);
            if (conversation is null)
                return TrainerErrors.ConversationNotFound();

            if (conversation.StudentUserId != userId && conversation.TrainerUserId != userId)
                return TrainerErrors.NotConversationParticipant();

            return null;
        }

        private async Task<ConversationDto> BuildConversationDtoAsync(
            Conversation conversation, string userId, CancellationToken ct)
        {
            var lastMessage = await dbContext.ChatMessages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.Id)
                .OrderByDescending(m => m.SentAtUtc)
                .FirstOrDefaultAsync(ct);

            var unreadCount = await dbContext.ChatMessages.CountAsync(m =>
                m.ConversationId == conversation.Id
                && m.SenderUserId != userId
                && m.ReadAtUtc == null, ct);

            return new ConversationDto
            {
                Id = conversation.Id,
                StudentUserId = conversation.StudentUserId,
                TrainerUserId = conversation.TrainerUserId,
                CreatedAtUtc = conversation.CreatedAtUtc,
                LastMessageAtUtc = conversation.LastMessageAtUtc,
                LastMessageText = lastMessage?.Text,
                UnreadCount = unreadCount
            };
        }
    }
}
