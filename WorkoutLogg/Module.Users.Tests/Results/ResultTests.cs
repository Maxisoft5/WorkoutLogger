using Modules.Common.Domain.Results;

namespace Module.Users.Tests.Results;

/// <summary>
/// Guards the Result/Result{T} success semantics. A regression here caused a
/// real bug: LoginAsync returned a Result with an EMPTY error list for wrong
/// credentials, which IsSuccess treats as success.
/// </summary>
[TestFixture]
public class ResultTests
{
    [Test]
    public void Result_WithError_IsNotSuccess()
    {
        var result = new Result(new Error("401", "Invalid credentials", ErrorType.Unauthorized));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Result_Success_IsSuccess()
    {
        Assert.That(Result.Success.IsSuccess, Is.True);
    }

    [Test]
    public void GenericResult_WithValue_IsSuccess()
    {
        var result = new Result<string>("value");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value"));
        });
    }

    [Test]
    public void GenericResult_WithError_IsNotSuccess()
    {
        var result = new Result<string>(new Error("404", "not found", ErrorType.NotFound));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsError, Is.True);
            Assert.That(result.Value, Is.Null);
            Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));
        });
    }

    [Test]
    public void GenericResult_WithNullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new Result<string>((string)null!));
    }

    /// <summary>
    /// Documents the dangerous edge case: an empty error list currently means
    /// success. Services must never signal a failure with an empty list —
    /// LoginAsync did exactly that before the fix.
    /// </summary>
    [Test]
    public void GenericResult_WithEmptyErrorList_IsTreatedAsSuccess_KnownPitfall()
    {
        var result = new Result<string>(new List<Error>());

        Assert.That(result.IsSuccess, Is.True,
            "Empty error list means success — never use it to signal a failure");
    }
}
