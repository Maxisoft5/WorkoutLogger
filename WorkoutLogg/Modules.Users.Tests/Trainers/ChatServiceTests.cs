using Microsoft.EntityFrameworkCore;
using Modules.Trainers.Infrastructure.Database;
using Modules.Trainers.Infrastructure.Domain;
using Modules.Trainers.Infrastructure.Services;

namespace Modules.Users.Tests.Trainers;

[TestFixture]
public class ChatServiceTests
{
    private TrainersDbContext _db = null!;
    private ChatService _service = null!;

    private const string Student = "student-1";
    private const string Trainer = "trainer-1";

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<TrainersDbContext>()
            .UseInMemoryDatabase($"chat-{Guid.NewGuid()}")
            .Options;
        _db = new TrainersDbContext(options);
        _service = new ChatService(_db);

        // Ученик и тренер связаны заявкой — чат разрешён.
        _db.TrainingRequests.Add(new TrainingRequest
        {
            Id = Guid.NewGuid(),
            StudentUserId = Student,
            TrainerUserId = Trainer,
            Status = TrainingRequestStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    private async Task<Guid> OpenConversationAsync(string byUser = Student, string with = Trainer)
    {
        var result = await _service.GetOrCreateConversationAsync(byUser, with);
        return result.Value!.Id;
    }

    [Test]
    public async Task OpenConversation_WithRelationship_CreatesOnce()
    {
        var first = await _service.GetOrCreateConversationAsync(Student, Trainer);
        var second = await _service.GetOrCreateConversationAsync(Trainer, Student); // с другой стороны

        Assert.Multiple(() =>
        {
            Assert.That(first.IsSuccess, Is.True);
            Assert.That(second.Value!.Id, Is.EqualTo(first.Value!.Id)); // тот же диалог
            Assert.That(_db.Conversations.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task OpenConversation_WithoutRelationship_IsRejected()
    {
        var result = await _service.GetOrCreateConversationAsync(Student, "stranger");

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NoChatRelationship"));
    }

    [Test]
    public async Task OpenConversation_WithSelf_IsRejected()
    {
        var result = await _service.GetOrCreateConversationAsync(Student, Student);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NoChatRelationship"));
    }

    [Test]
    public async Task SendMessage_ByParticipant_UpdatesLastMessage()
    {
        var conversationId = await OpenConversationAsync();

        var sent = await _service.SendMessageAsync(Student, conversationId, "Здравствуйте! Когда можно начать?");

        Assert.Multiple(() =>
        {
            Assert.That(sent.IsSuccess, Is.True);
            Assert.That(_db.Conversations.Single().LastMessageAtUtc, Is.Not.Null);
        });
    }

    [Test]
    public async Task SendMessage_ByOutsider_IsForbidden()
    {
        var conversationId = await OpenConversationAsync();

        var result = await _service.SendMessageAsync("stranger", conversationId, "спам");

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NotConversationParticipant"));
    }

    [TestCase("")]
    [TestCase("   ")]
    public async Task SendMessage_EmptyText_IsRejected(string text)
    {
        var conversationId = await OpenConversationAsync();

        var result = await _service.SendMessageAsync(Student, conversationId, text);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.EmptyMessage"));
    }

    [Test]
    public async Task GetMessages_PagesNewestFirst_ChronologicalInsidePage()
    {
        var conversationId = await OpenConversationAsync();
        for (var i = 1; i <= 5; i++)
        {
            await _service.SendMessageAsync(Student, conversationId, $"msg-{i}");
            await Task.Delay(5);
        }

        var page1 = await _service.GetMessagesAsync(Student, conversationId, page: 1, pageSize: 2);

        Assert.Multiple(() =>
        {
            Assert.That(page1.Value!.TotalCount, Is.EqualTo(5));
            Assert.That(page1.Value.Items.Select(m => m.Text), Is.EqualTo(new[] { "msg-4", "msg-5" }));
        });
    }

    [Test]
    public async Task GetMessages_ByOutsider_IsForbidden()
    {
        var conversationId = await OpenConversationAsync();

        var result = await _service.GetMessagesAsync("stranger", conversationId, 1, 10);

        Assert.That(result.Errors![0].Code, Is.EqualTo("Trainers.NotConversationParticipant"));
    }

    [Test]
    public async Task UnreadCount_And_MarkRead_Flow()
    {
        var conversationId = await OpenConversationAsync();
        await _service.SendMessageAsync(Student, conversationId, "Привет!");
        await _service.SendMessageAsync(Student, conversationId, "Вы свободны завтра?");

        var trainerView = (await _service.GetConversationsAsync(Trainer)).Single();
        var studentView = (await _service.GetConversationsAsync(Student)).Single();
        var marked = await _service.MarkReadAsync(Trainer, conversationId);
        var trainerViewAfter = (await _service.GetConversationsAsync(Trainer)).Single();

        Assert.Multiple(() =>
        {
            Assert.That(trainerView.UnreadCount, Is.EqualTo(2));
            Assert.That(trainerView.LastMessageText, Is.EqualTo("Вы свободны завтра?"));
            Assert.That(studentView.UnreadCount, Is.EqualTo(0)); // свои сообщения не считаются
            Assert.That(marked.Value, Is.EqualTo(2));
            Assert.That(trainerViewAfter.UnreadCount, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task Conversations_OrderedByLastActivity()
    {
        // вторая пара с заявкой
        _db.TrainingRequests.Add(new TrainingRequest
        {
            Id = Guid.NewGuid(),
            StudentUserId = "student-2",
            TrainerUserId = Trainer,
            Status = TrainingRequestStatus.Accepted,
            CreatedAtUtc = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var conv1 = await OpenConversationAsync();
        var conv2Result = await _service.GetOrCreateConversationAsync("student-2", Trainer);
        await _service.SendMessageAsync("student-2", conv2Result.Value!.Id, "hi");
        await Task.Delay(5);
        await _service.SendMessageAsync(Student, conv1, "now newer");

        var list = await _service.GetConversationsAsync(Trainer);

        Assert.Multiple(() =>
        {
            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].Id, Is.EqualTo(conv1));
        });
    }
}
