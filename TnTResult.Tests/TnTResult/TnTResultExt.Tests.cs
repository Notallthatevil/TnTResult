using TnTResult.Ext;
using System.Threading;

namespace TnTResult_Tests;

public class TnTResultExtTests {

    [Fact]
    public async Task OnFailureAsync_Task_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        Exception? captured = null;
        var r = await Task.FromResult(result).OnFailureAsync(e => captured = e);
        captured.Should().Be(ex);
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnFailureAsync_Task_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        var called = false;
        var r = await Task.FromResult(result).OnFailureAsync(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_Task_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync(() => called = true);
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_Task_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnFailureAsync_ValueTask_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        Exception? captured = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnFailureAsync(e => captured = e);
        captured.Should().Be(ex);
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnFailureAsync_ValueTask_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure(ex);
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnFailureAsync(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnFailureAsync_TaskT_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        Exception? captured = null;
        var r = await Task.FromResult(result).OnFailureAsync<string>(e => captured = e);
        captured.Should().Be(ex);
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnFailureAsync_ValueTaskT_Action_CalledOnFailure() {
        var ex = new InvalidOperationException("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        Exception? captured = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnFailureAsync<string>(e => captured = e);
        captured.Should().Be(ex);
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnFailureAsync_TaskT_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        var called = false;
        var r = await Task.FromResult(result).OnFailureAsync<string>(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnFailureAsync_ValueTaskT_Func_CalledOnFailure() {
        var ex = new Exception("fail");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnFailureAsync<string>(async e => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTask_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnSuccessAsync(() => called = true);
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTask_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult>(result).OnSuccessAsync(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(() => called = true);
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_Action_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(() => called = true);
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_Func_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        var called = false;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(async () => { called = true; await Task.Yield(); });
        called.Should().BeTrue();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_ActionT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(v => value = v);
        value.Should().Be("ok");
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_ActionT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(v => value = v);
        value.Should().Be("ok");
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskT_FuncT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await Task.FromResult(result).OnSuccessAsync<string>(async v => { value = v; await Task.Yield(); });
        value.Should().Be("ok");
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_ValueTaskT_FuncT_CalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Success("ok");
        string? value = null;
        var r = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).OnSuccessAsync<string>(async v => { value = v; await Task.Yield(); });
        value.Should().Be("ok");
        r.Should().Be(result);
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

    // Added tests for negative scenarios and async helpers

    [Fact]
    public async Task OnFailureAsync_Task_Action_NotCalledOnSuccess() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        var r = await Task.FromResult(result).OnFailureAsync(_ => called = true);
        called.Should().BeFalse();
        r.Should().Be(result);
    }

    [Fact]
    public async Task OnSuccessAsync_Task_Action_NotCalledOnFailure() {
        var result = global::TnTResult.TnTResult.Failure(new InvalidOperationException("fail"));
        var called = false;
        var r = await Task.FromResult(result).OnSuccessAsync(() => called = true);
        called.Should().BeFalse();
        r.Should().Be(result);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_Task_Success_ReturnsValue() {
        var result = Task.FromResult<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Success("v"));
        var v = await result.GetValueOrDefaultAsync();
        v.Should().Be("v");
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_Task_Failure_ReturnsDefault() {
        var result = Task.FromResult<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(new Exception("e")));
        var v = await result.GetValueOrDefaultAsync();
        v.Should().BeNull();
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_ValueTask_Success_ReturnsValue() {
        var result = new ValueTask<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Success("v"));
        var v = await result.GetValueOrDefaultAsync();
        v.Should().Be("v");
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_ValueTask_Failure_ReturnsDefault() {
        var result = new ValueTask<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(new Exception("e")));
        var v = await result.GetValueOrDefaultAsync();
        v.Should().BeNull();
    }

    [Fact]
    public async Task ValueOrAsync_Task_Success_ReturnsValue() {
        var result = Task.FromResult<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Success("v"));
        var v = await result.ValueOrAsync("d");
        v.Should().Be("v");
    }

    [Fact]
    public async Task ValueOrAsync_Task_Failure_ReturnsDefault() {
        var result = Task.FromResult<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(new Exception("e")));
        var v = await result.ValueOrAsync("d");
        v.Should().Be("d");
    }

    [Fact]
    public async Task ValueOrAsync_ValueTask_Success_ReturnsValue() {
        var result = new ValueTask<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Success("v"));
        var v = await result.ValueOrAsync("d");
        v.Should().Be("v");
    }

    [Fact]
    public async Task ValueOrAsync_ValueTask_Failure_ReturnsDefault() {
        var result = new ValueTask<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(new Exception("e")));
        var v = await result.ValueOrAsync("d");
        v.Should().Be("d");
    }

    [Fact]
    public async Task ValueOrThrowAsync_Task_Success_ReturnsValue() {
        var result = Task.FromResult<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Success("v"));
        var v = await result.ValueOrThrowAsync();
        v.Should().Be("v");
    }

    [Fact]
    public async Task ValueOrThrowAsync_Task_Failure_ThrowsOriginal() {
        var ex = new InvalidOperationException("boom");
        var result = Task.FromResult<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(ex));
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => result.ValueOrThrowAsync());
        thrown.Should().BeSameAs(ex);
    }

    [Fact]
    public async Task ValueOrThrowAsync_Task_Failure_ThrowsCustom() {
        var result = Task.FromResult<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(new Exception("e")));
        var thrown = await Assert.ThrowsAsync<ApplicationException>(() => result.ValueOrThrowAsync(() => new ApplicationException("c")));
        thrown.Message.Should().Be("c");
    }

    [Fact]
    public async Task ValueOrThrowAsync_ValueTask_Success_ReturnsValue() {
        var result = new ValueTask<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Success("v"));
        var v = await result.ValueOrThrowAsync();
        v.Should().Be("v");
    }

    [Fact]
    public async Task ValueOrThrowAsync_ValueTask_Failure_ThrowsOriginal() {
        var ex = new InvalidOperationException("boom");
        var result = new ValueTask<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(ex));
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(async () => _ = await result.ValueOrThrowAsync());
        thrown.Should().BeSameAs(ex);
    }

    [Fact]
    public async Task ValueOrThrowAsync_ValueTask_Failure_ThrowsCustom() {
        var result = new ValueTask<global::TnTResult.ITnTResult<string>>(global::TnTResult.TnTResult.Failure<string>(new Exception("e")));
        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () => _ = await result.ValueOrThrowAsync(() => new ApplicationException("c")));
        thrown.Message.Should().Be("c");
    }

    [Fact]
    public async Task ThrowOnFailureAsync_Task_NonGeneric_Success_ReturnsSameInstance() {
        var result = global::TnTResult.TnTResult.Successful;
        var returned = await Task.FromResult(result).ThrowOnFailureAsync();
        returned.Should().Be(result);
    }

    [Fact]
    public async Task ThrowOnFailureAsync_Task_NonGeneric_Failure_ThrowsOriginal() {
        var ex = new InvalidOperationException("boom");
        var result = global::TnTResult.TnTResult.Failure(ex);
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.FromResult(result).ThrowOnFailureAsync());
        thrown.Should().BeSameAs(ex);
    }

    [Fact]
    public async Task ThrowOnFailureAsync_Task_NonGeneric_Failure_ThrowsCustom() {
        var result = global::TnTResult.TnTResult.Failure(new Exception("e"));
        var thrown = await Assert.ThrowsAsync<ApplicationException>(() => Task.FromResult(result).ThrowOnFailureAsync(() => new ApplicationException("c")));
        thrown.Message.Should().Be("c");
    }

    [Fact]
    public async Task ThrowOnFailureAsync_Task_Generic_Success_ReturnsSameInstance() {
        var result = global::TnTResult.TnTResult.Success("v");
        var returned = await Task.FromResult(result).ThrowOnFailureAsync();
        returned.Should().Be(result);
    }

    [Fact]
    public async Task ThrowOnFailureAsync_Task_Generic_Failure_ThrowsOriginal() {
        var ex = new InvalidOperationException("boom");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.FromResult(result).ThrowOnFailureAsync());
        thrown.Should().BeSameAs(ex);
    }

    [Fact]
    public async Task ThrowOnFailureAsync_Task_Generic_Failure_ThrowsCustom() {
        var result = global::TnTResult.TnTResult.Failure<string>(new Exception("e"));
        var thrown = await Assert.ThrowsAsync<ApplicationException>(() => Task.FromResult(result).ThrowOnFailureAsync(() => new ApplicationException("c")));
        thrown.Message.Should().Be("c");
    }

    [Fact]
    public async Task ThrowOnFailureAsync_ValueTask_NonGeneric_Success_ReturnsSameInstance() {
        var result = global::TnTResult.TnTResult.Successful;
        var returned = await new ValueTask<global::TnTResult.ITnTResult>(result).ThrowOnFailureAsync();
        returned.Should().Be(result);
    }

    [Fact]
    public async Task ThrowOnFailureAsync_ValueTask_NonGeneric_Failure_ThrowsOriginal() {
        var ex = new InvalidOperationException("boom");
        var result = global::TnTResult.TnTResult.Failure(ex);
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => new ValueTask<global::TnTResult.ITnTResult>(result).ThrowOnFailureAsync().AsTask());
        thrown.Should().BeSameAs(ex);
    }

    [Fact]
    public async Task ThrowOnFailureAsync_ValueTask_Generic_Success_ReturnsSameInstance() {
        var result = global::TnTResult.TnTResult.Success("v");
        var returned = await new ValueTask<global::TnTResult.ITnTResult<string>>(result).ThrowOnFailureAsync();
        returned.Should().Be(result);
    }

    [Fact]
    public async Task ThrowOnFailureAsync_ValueTask_Generic_Failure_ThrowsOriginal() {
        var ex = new InvalidOperationException("boom");
        var result = global::TnTResult.TnTResult.Failure<string>(ex);
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => new ValueTask<global::TnTResult.ITnTResult<string>>(result).ThrowOnFailureAsync().AsTask());
        thrown.Should().BeSameAs(ex);
    }

    [Fact]
    public async Task FinallyAsync_Task_Action_Called() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        await Task.FromResult(result).FinallyAsync(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_ValueTask_Action_Called() {
        var result = global::TnTResult.TnTResult.Successful;
        var called = false;
        await new ValueTask<global::TnTResult.ITnTResult>(result).FinallyAsync(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_TaskT_Action_Called() {
        var result = global::TnTResult.TnTResult.Success("v");
        var called = false;
        await Task.FromResult(result).FinallyAsync<string>(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_ValueTaskT_Action_Called() {
        var result = global::TnTResult.TnTResult.Success("v");
        var called = false;
        await new ValueTask<global::TnTResult.ITnTResult<string>>(result).FinallyAsync<string>(() => called = true);
        called.Should().BeTrue();
    }

    [Fact]
    public async Task FinallyAsync_Task_WithCancellation_ThrowsAndCallsAction() {
        var tcs = new TaskCompletionSource<global::TnTResult.ITnTResult>();
        var cts = new CancellationTokenSource();
        var called = false;
        cts.Cancel();
        var ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => tcs.Task.FinallyAsync(() => called = true, cts.Token));
        ex.Should().NotBeNull();
        called.Should().BeTrue();
    }

    [Fact]
    public async Task ValueOrThrowAsync_Task_WithCancellation_Throws() {
        var tcs = new TaskCompletionSource<global::TnTResult.ITnTResult<string>>();
        var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => tcs.Task.ValueOrThrowAsync(() => new ApplicationException("unused"), cts.Token));
    }
}