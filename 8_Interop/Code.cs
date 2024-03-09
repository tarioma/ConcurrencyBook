using System.Net;
using System.Threading.Tasks.Dataflow;

namespace _8_Interop;

public static class P1
{
    public static Task<string> DownloadStringTaskAsync(this WebClient client,
        Uri address)
    {
        var tcs = new TaskCompletionSource<string>();

        // The event handler will complete the task and unregister itself.
        DownloadStringCompletedEventHandler handler = null;
        handler = (_, e) =>
        {
            client.DownloadStringCompleted -= handler;
            if (e.Cancelled)
                tcs.TrySetCanceled();
            else if (e.Error != null)
                tcs.TrySetException(e.Error);
            else
                tcs.TrySetResult(e.Result);
        };

        // Register for the event and *then* start the operation.
        client.DownloadStringCompleted += handler;
        client.DownloadStringAsync(address);

        return tcs.Task;
    }
}

public static class P2
{
    public static Task<WebResponse> GetResponseAsync(this WebRequest client)
    {
        return Task<WebResponse>.Factory.FromAsync(client.BeginGetResponse,
            client.EndGetResponse, null);
    }
}

public static class P3
{
    public interface IMyAsyncHttpService
    {
        void DownloadString(Uri address, Action<string, Exception> callback);
    }

    public static Task<string> DownloadStringAsync(
        this IMyAsyncHttpService httpService, Uri address)
    {
        var tcs = new TaskCompletionSource<string>();
        httpService.DownloadString(address, (result, exception) =>
        {
            if (exception != null)
                tcs.TrySetException(exception);
            else
                tcs.TrySetResult(result);
        });
        return tcs.Task;
    }
}

public class P4
{
    public async Task Test()
    {
        var source = Enumerable.Range(0, 10);
        Action<int> body = x => { };

        await Task.Run(() => Parallel.ForEach(source, body));
    }
}

public class P7A
{
    public async Task Test()
    {
        var multiplyBlock = new TransformBlock<int, int>(value => value * 2);

        multiplyBlock.Post(5);
        multiplyBlock.Post(2);
        multiplyBlock.Complete();

        await foreach (int item in multiplyBlock.ReceiveAllAsync())
        {
            Console.WriteLine(item);
        }
    }
}

public static class P7B
{
    public static async Task WriteToBlockAsync<T>(this IAsyncEnumerable<T> enumerable,
        ITargetBlock<T> block, CancellationToken token = default)
    {
        try
        {
            await foreach (var item in enumerable.WithCancellation(token).ConfigureAwait(false))
                await block.SendAsync(item, token).ConfigureAwait(false);
            block.Complete();
        }
        catch (Exception ex)
        {
            block.Fault(ex);
        }
    }
}