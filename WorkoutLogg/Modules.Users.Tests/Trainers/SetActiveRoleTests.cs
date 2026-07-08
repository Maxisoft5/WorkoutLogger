using Modules.Common.Domain.Results;
using Modules.Users.DTO.Users;
using Modules.Users.Infrastructure.Authorization;

namespace Modules.Users.Tests.Trainers;

// UsersDbContext не тестируется против EF InMemory/SQLite (Postgres-специфичная модель:
// complex property BodyStats, default SQL) — как и в AuthServiceTests, проверяем только
// логику, не требующую базы.
[TestFixture]
public class SetActiveRoleTests
{
    [Test]
    public async Task SetActiveRole_UndefinedEnumValue_ReturnsValidationErrorWithoutDbAccess()
    {
        var service = new UserService(null!);

        var result = await service.SetActiveRoleAsync("user-1", (AccountRole)42);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Errors![0].Type, Is.EqualTo(ErrorType.Validation));
            Assert.That(result.Errors![0].Code, Is.EqualTo("Users.InvalidRole"));
        });
    }

    [Test]
    public void AccountRole_DefaultValue_IsStudent()
    {
        Assert.That(default(AccountRole), Is.EqualTo(AccountRole.Student));
    }
}
