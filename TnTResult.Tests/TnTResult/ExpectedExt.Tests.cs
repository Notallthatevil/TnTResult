using TnTResult;
using TnTResult.Ext;

namespace TnTResult_Tests;

public class ExpectedExtTests {

    [Fact]
    public async Task AndThenAsync_Task_TransformsValue() {
        var exp = Expected.MakeExpected<string, bool>("abc");
        var result = await Task.FromResult(exp).AndThenAsync(x => x.Length);
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(3);
    }

    [Fact]
    public async Task OrElseAsync_Task_TransformsError() {
        var exp = Expected.MakeUnexpected<bool, string>("err");
        var result = await Task.FromResult(exp).OrElseAsync(e => e.Length);
        result.HasValue.Should().BeFalse();
        result.Error.Should().Be(3);
    }

    [Fact]
    public async Task TransformAsync_Task_TransformsValue() {
        var exp = Expected.MakeExpected<int, bool>(7);
        var result = await Task.FromResult(exp).TransformAsync(x => x.ToString());
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("7");
    }

    [Fact]
    public async Task TransformErrorAsync_Task_TransformsError() {
        var exp = Expected.MakeUnexpected<bool, string>("fail");
        var result = await Task.FromResult(exp).TransformErrorAsync(e => e.Length);
        result.HasValue.Should().BeFalse();
        result.Error.Should().Be(4);
    }

    [Fact]
    public async Task ValueOrAsync_Task_ReturnsValueOrDefault() {
        var exp = Expected.MakeUnexpected<int, string>("fail");
        var value = await Task.FromResult(exp).ValueOrAsync(99);
        value.Should().Be(99);
    }
}