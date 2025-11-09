using Xunit.Sdk;

namespace Core.Tests.Utils;

public static class CancellationTestHelper
{
    public static async Task ShouldRespectCancellationAsync(Func<CancellationToken, Task> asyncAction)
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        try
        {
            await asyncAction(cts.Token);
        }
        catch (Exception ex)
        {
            Assert.True(ex is OperationCanceledException or TaskCanceledException);
            return;
        }

        throw new XunitException("Expected operation cancelled, but no exception was thrown.");
    }

    public static async Task ShouldRespectCancellationAsync<T>(
        Func<CancellationToken, IAsyncEnumerable<T>> asyncEnumerable)
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        try
        {
            await foreach (var item in asyncEnumerable(cts.Token)) ;
        }
        catch (Exception ex)
        {
            Assert.True(ex is OperationCanceledException or TaskCanceledException);
            return;
        }

        throw new XunitException("Expected operation cancelled, but no exception was thrown.");
    }

    public static async Task ShouldAllRespectCancellationAsync(List<Func<CancellationToken, Task>> asyncActions)
    {
        foreach (var action in asyncActions)
        {
            await ShouldRespectCancellationAsync(action);
        }
    }

    public static async Task ShouldAllRespectCancellationAsync<T>(
        List<Func<CancellationToken, IAsyncEnumerable<T>>> asyncActions)
    {
        foreach (var action in asyncActions)
        {
            await ShouldRespectCancellationAsync(action);
        }
    }
}