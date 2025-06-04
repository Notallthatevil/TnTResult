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

    [Fact]
    public async Task OnFailureAsync_ValueTask_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        Exception? captured = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnFailureAsync(e => captured = e);
        captured.Should().Be(ex);
    }

    [Fact]
    public async Task OnFailureAsync_ValueTask_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnFailureAsync(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }

    [Fact]
    public async Task OnFailureAsync_TaskT_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        Exception? captured = null;
        var r = await Task.FromResult(result).OnFailureAsync<string>(e => captured = e);
        captured.Should().Be(ex);
    }

    [Fact]
    public async Task OnFailureAsync_ValueTaskT_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        Exception? captured = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnFailureAsync<string>(e => captured = e);
        captured.Should().Be(ex);
    }

    [Fact]
    public async Task OnFailureAsync_TaskT_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        var called = false;
        var r = await Task.FromResult(result).OnFailureAsync<string>(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }

    [Fact]
    public async Task OnFailureAsync_ValueTaskT_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnFailureAsync<string>(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTask_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnSuccessAsync(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTask_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnSuccessAsync(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().BeSameAs(result);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_ActionT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(v => value = v);
        value.Should().Be("ok");
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_ActionT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(v => value = v);
        value.Should().Be("ok");
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_FuncT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(async v => { value = v; await Task.Yield(); });
        value.Should().Be("ok");
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_FuncT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(async v => { value = v; await Task.Yield(); });
        value.Should().Be("ok");
    }

    [Fact]
    public async Task FinallyAsync_Task_CalledRegardlessOfResult()
    {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        await Task.FromResult(result).FinallyAsync(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_Task_CalledOnFailure()
    {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        var called = false;
        await Task.FromResult(result).FinallyAsync(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_ValueTask_CalledRegardlessOfResult()
    {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        await new ValueTask<global::TnTResult.ITnTResult>(result).FinallyAsync(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_TaskT_CalledRegardlessOfResult()
    {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        await Task.FromResult(result).FinallyAsync<string>(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_ValueTaskT_CalledRegardlessOfResult()
    {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        await new ValueTask<global::TnTResult.ITnTResult<string>>(result).FinallyAsync<string>(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
    }
}