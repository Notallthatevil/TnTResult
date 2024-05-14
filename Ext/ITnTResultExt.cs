using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TnTResult;
public static class ITnTResultExt {
    /// <summary>
    /// Executes the specified action if the result is a failure.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns><paramref name="tntResult"/></returns>
    public static ITnTResult OnFailure(this ITnTResult tntResult, Action<Exception> action) => tntResult.OnFailureAsync((exc) => {
        action(exc);
        return Task.CompletedTask;
    }).Result;
    /// <summary>
    /// Asynchronously executes the specified function if the result is a failure.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation containing the provided <paramref name="tntResult"/></returns>
    public static async Task<ITnTResult> OnFailureAsync(this ITnTResult tntResult, Func<Exception, Task> func) {
        if (!tntResult.IsSuccessful) {
            await func(tntResult.Error);
        }
        return tntResult;
    }
    /// <summary>
    /// Executes the specified action if the result is a success.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns><paramref name="tntResult"/></returns>
    public static ITnTResult OnSuccess(this ITnTResult tntResult, Action action) {
        return tntResult.OnSuccessAsync(() => {
            action();
            return Task.CompletedTask;
        }).Result;
    }
    /// <summary>
    /// Asynchronously executes the specified function if the result is a success.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation containing the provided <paramref name="tntResult"/></returns>
    public static async Task<ITnTResult> OnSuccessAsync(this ITnTResult tntResult, Func<Task> func) {
        if (tntResult.IsSuccessful) {
            await func();
        }
        return tntResult;
    }

    /// <summary>
    /// Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns><paramref name="tntResult"/></returns>
    public static ITnTResult<TSuccess> OnSuccess<TSuccess>(this ITnTResult<TSuccess> tntResult, Action<TSuccess?> action) => tntResult.OnSuccessAsync((value) => {
        action(value);
        return Task.CompletedTask;
    }).Result;

    /// <summary>
    /// Executes the specified asynchronous function if the operation was successful.
    /// </summary>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation containing the provided <paramref name="tntResult"/></returns>
    public static async Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ITnTResult<TSuccess> tntResult, Func<TSuccess?, Task> func) {
        if (tntResult.IsSuccessful) {
            await func(tntResult.Value);
        }
        return tntResult;
    }

}

