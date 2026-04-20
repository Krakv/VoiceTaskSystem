using TaskManager.Shared.Pipeline;

namespace TaskManager.UnitTests.Shared;

public class SimpleValidatorsTests
{
    // ---------- GUID ----------

    [Fact]
    public void BeValidGuid_Should_Return_True_For_Valid()
    {
        var result = SimpleValidators.BeValidGuid(Guid.NewGuid().ToString());

        Assert.True(result);
    }

    [Fact]
    public void BeValidGuid_Should_Return_False_For_Invalid()
    {
        var result = SimpleValidators.BeValidGuid("invalid");

        Assert.False(result);
    }

    [Fact]
    public void BeValidGuidOrNull_Should_Return_True_For_Null()
    {
        Assert.True(SimpleValidators.BeValidGuidOrNull(null));
    }

    [Fact]
    public void BeValidGuidOrNull_Should_Return_True_For_Empty()
    {
        Assert.True(SimpleValidators.BeValidGuidOrNull(""));
    }

    [Fact]
    public void BeValidGuidOrNull_Should_Return_False_For_Invalid()
    {
        Assert.False(SimpleValidators.BeValidGuidOrNull("bad-guid"));
    }

    // ---------- DateTime ----------

    [Fact]
    public void BeValidDateTimeOffset_Should_Return_True_For_Valid()
    {
        var result = SimpleValidators.BeValidDateTimeOffset("2025-01-01T10:00:00Z");

        Assert.True(result);
    }

    [Fact]
    public void BeValidDateTimeOffset_Should_Return_False_For_Invalid()
    {
        var result = SimpleValidators.BeValidDateTimeOffset("not-a-date");

        Assert.False(result);
    }

    [Fact]
    public void BeValidDateTimeOffsetOrNull_Should_Return_True_For_Null()
    {
        Assert.True(SimpleValidators.BeValidDateTimeOffsetOrNull(null));
    }

    [Fact]
    public void BeValidDateTimeOffsetOrNull_Should_Return_False_For_Invalid()
    {
        Assert.False(SimpleValidators.BeValidDateTimeOffsetOrNull("bad-date"));
    }

    // ---------- ENUM ----------

    private enum TestEnum { One, Two }

    [Fact]
    public void BeValidEnum_Should_Return_True_For_Valid()
    {
        var result = SimpleValidators.BeValidEnum<TestEnum>("One");

        Assert.True(result);
    }

    [Fact]
    public void BeValidEnum_Should_Be_Case_Insensitive()
    {
        var result = SimpleValidators.BeValidEnum<TestEnum>("one");

        Assert.True(result);
    }

    [Fact]
    public void BeValidEnum_Should_Return_False_For_Invalid()
    {
        var result = SimpleValidators.BeValidEnum<TestEnum>("Three");

        Assert.False(result);
    }

    [Fact]
    public void BeValidEnumOrNull_Should_Return_True_For_Null()
    {
        Assert.True(SimpleValidators.BeValidEnumOrNull<TestEnum>(null));
    }

    // ---------- INT ----------

    [Fact]
    public void BeValidIntPositive_Should_Return_True_For_Positive()
    {
        Assert.True(SimpleValidators.BeValidIntPositive("10"));
    }

    [Fact]
    public void BeValidIntPositive_Should_Return_False_For_Zero()
    {
        Assert.False(SimpleValidators.BeValidIntPositive("0"));
    }

    [Fact]
    public void BeValidIntNonNegative_Should_Return_True_For_Zero()
    {
        Assert.True(SimpleValidators.BeValidIntNonNegative("0"));
    }

    [Fact]
    public void BeValidIntNonNegative_Should_Return_False_For_Negative()
    {
        Assert.False(SimpleValidators.BeValidIntNonNegative("-1"));
    }

    // ---------- JSON ----------

    [Fact]
    public void BeValidJson_Should_Return_True_For_Valid()
    {
        var json = "{\"key\":\"value\"}";

        Assert.True(SimpleValidators.BeValidJson(json));
    }

    [Fact]
    public void BeValidJson_Should_Return_False_For_Invalid()
    {
        var json = "{invalid json}";

        Assert.False(SimpleValidators.BeValidJson(json));
    }

    [Fact]
    public void BeValidJsonOrNull_Should_Return_True_For_Null()
    {
        Assert.True(SimpleValidators.BeValidJsonOrNull(null));
    }

    [Fact]
    public void BeValidJsonOrNull_Should_Return_False_For_Invalid()
    {
        Assert.False(SimpleValidators.BeValidJsonOrNull("{bad json}"));
    }
}
