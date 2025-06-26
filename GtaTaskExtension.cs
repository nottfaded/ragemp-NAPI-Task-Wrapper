internal static class GtaTaskExtension
{
    public static void SafeRun(this GTANetworkMethods.Task task, Action action)
    {
        if (Thread.CurrentThread.ManagedThreadId == NAPI.MainThreadId)
            action.Invoke();
        else NAPI.Task.Run(action.Invoke);
    }
    public static Task<T> SafeRun<T>(this GTANetworkMethods.Task task, Func<T> func)
    {
        if (Thread.CurrentThread.ManagedThreadId == NAPI.MainThreadId)
        {
            try
            {
                return Task.FromResult(func());
            }
            catch (Exception e)
            {
                return Task.FromException<T>(e);
            }
        }

        var tcs = new TaskCompletionSource<T>();

        NAPI.Task.SafeRun(() =>
        {
            try
            {
                var result = func();
                tcs.SetResult(result);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return tcs.Task;
    }

    public static async Task ReturnToMainThread(this GTANetworkMethods.Task task)
        => await NAPI.Task.SafeRun(() => Task.CompletedTask);
}

internal static class TaskExtension
{
    public static async Task<T> ReturnToMainThread<T>(this Task<T> task)
    {
        try
        {
            var result = await task;
            return result;
        }
        finally
        {
            await NAPI.Task.ReturnToMainThread();
        }
    }

    public static async Task ReturnToMainThread(this Task task)
    {
        try
        {
            await task;
        }
        finally
        {
            await NAPI.Task.ReturnToMainThread();
        }
    }
}