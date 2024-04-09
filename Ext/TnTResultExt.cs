namespace TnTResult.Ext;

public static class TnTResultExt {

    public static ITnTResult<TSuccess> MakeSuccessful<TSuccess>(this TSuccess obj) {
        return ITnTResult<TSuccess>.Success(obj);
    }

    public static async ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Action<Exception> action) {
        return (await result).OnFailure(action);
    }

    public static async Task<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception, Task> func) {
        return await (await result).OnFailureAsync(func);
    }

    public static async ValueTask<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Action<Exception> action) {
        return (await result).OnFailure(action);
    }

    public static async Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Func<Exception, Task> func) {
        return await (await result).OnFailureAsync(func);
    }

    public static async ValueTask<ITnTResult> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<Exception> action) {
        return (await result).OnFailure(action);
    }

    public static async Task<ITnTResult> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception, Task> func) {
        return await (await result).OnFailureAsync(func);
    }

    public static async ValueTask<ITnTResult> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<Exception> action) {
        return (await result).OnFailure(action);
    }

    public static async Task<ITnTResult> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception, Task> func) {
        return await (await result).OnFailureAsync(func);
    }

    public static async ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Action action) {
        return (await result).OnSuccess(action);
    }

    public static async Task<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Func<Task> func) {
        return await (await result).OnSuccessAsync(func);
    }

    public static async ValueTask<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Action action) {
        return (await result).OnSuccess(action);
    }

    public static async Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Func<Task> func) {
        return await (await result).OnSuccessAsync(func);
    }

    public static async ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<TSuccess?> action) {
        return (await result).OnSuccess(action);
    }

    public static async Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<TSuccess?, Task> func) {
        return await (await result).OnSuccessAsync(func);
    }

    public static async ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<TSuccess?> action) {
        return (await result).OnSuccess(action);
    }

    public static async Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<TSuccess?, Task> func) {
        return await (await result).OnSuccessAsync(func);
    }
}