using Xunit;
using AwesomeAssertions;
using TnTResult.Ext;
using TnTResult;

namespace TnTResult_Tests;
public class OptionalExtTests {
    [Fact]
    public async Task AndThenAsync_Task_Action_CalledOnValue() {
        var opt = Optional.MakeOptional(42);
        var called = false;
        var result = await Task.FromResult(opt).AndThenAsync(x => called = true);
        called.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task AndThenAsync_ValueTask_Action_CalledOnValue() {
        var opt = Optional.MakeOptional("abc");
        var called = false;
        var result = await new ValueTask<Optional<string>>(opt).AndThenAsync(x => called = true);
        called.Should().BeTrue();
        result.Value.Should().Be("abc");
    }

    [Fact]
    public async Task OrElseAsync_Task_Action_CalledOnNone() {
        var opt = Optional<string>.NullOpt;
        var called = false;
        var result = await Task.FromResult(opt).OrElseAsync(() => called = true);
        called.Should().BeTrue();
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task OrElseAsync_ValueTask_Action_CalledOnNone() {
        var opt = Optional<string>.NullOpt;
        var called = false;
        var result = await new ValueTask<Optional<string>>(opt).OrElseAsync(() => called = true);
        called.Should().BeTrue();
        result.HasValue.Should().BeFalse();
    }
}

