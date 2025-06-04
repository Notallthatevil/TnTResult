using TnTResult.Ext;

namespace TnTResult_Tests;

public class TnTResultExtTests {

    [Fact]
    public async Task OnFailureAsync_Task_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        Exception? captured = null;
        var r = await Task.FromResult(result).OnFailureAsync(e => captured = e);
        captured.Should().Be(ex);
    }

    [Fact]
    public async Task OnFailureAsync_Task_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        var called = false;
        var r = await Task.FromResult(result).OnFailureAsync(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }

    [Fact]
    public async Task OnSuccessAsync_Task_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task OnSuccessAsync_Task_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }
}