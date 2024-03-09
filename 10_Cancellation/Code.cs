using System.Net.NetworkInformation;
using System.Threading.Tasks.Dataflow;

namespace _10_Cancellation;

public class P1
{
    public async Task<int> CancelableMethodAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        return 42;
    }

    public void IssueCancelRequest()
    {
        using var cts = new CancellationTokenSource();
        var task = CancelableMethodAsync(cts.Token);

        // At this point, the operation has been started.

        // Issue the cancellation request.
        cts.Cancel();
    }

    public async Task IssueCancelRequestAsync()
    {
        using var cts = new CancellationTokenSource();
        var task = CancelableMethodAsync(cts.Token);

        // At this point, the operation is happily running.

        // Issue the cancellation request.
        cts.Cancel();

        // (Asynchronously) wait for the operation to finish.
        try
        {
            await task;
            // If we get here, the operation completed successfully
            //  before the cancellation took effect.
        }
        catch (OperationCanceledException)
        {
            // If we get here, the operation was canceled before it completed.
        }
        catch (Exception)
        {
            // If we get here, the operation completed with an error
            //  before the cancellation took effect.
            throw;
        }
    }
}

public class P2A
{
    public int CancelableMethod(CancellationToken cancellationToken)
    {
        for (int i = 0; i != 100; ++i)
        {
            Thread.Sleep(1000); // Some calculation goes here.
            cancellationToken.ThrowIfCancellationRequested();
        }

        return 42;
    }
}

public class P2B
{
    public int CancelableMethod(CancellationToken cancellationToken)
    {
        for (int i = 0; i != 100000; ++i)
        {
            Thread.Sleep(1); // Some calculation goes here.
            if (i % 1000 == 0)
                cancellationToken.ThrowIfCancellationRequested();
        }

        return 42;
    }
}

public class P3A
{
    public async Task IssueTimeoutAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        CancellationToken token = cts.Token;
        await Task.Delay(TimeSpan.FromSeconds(10), token);
    }
}

public class P3B
{
    public async Task IssueTimeoutAsync()
    {
        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        cts.CancelAfter(TimeSpan.FromSeconds(5));
        await Task.Delay(TimeSpan.FromSeconds(10), token);
    }
}

public class P4
{
    public async Task<int> CancelableMethodAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        return 42;
    }
}

public class P5
{
    public abstract class Matrix
    {
        public abstract void Rotate(float degrees);
    }

    public void RotateMatrices(IEnumerable<Matrix> matrices, float degrees,
        CancellationToken token)
    {
        Parallel.ForEach(matrices,
            new ParallelOptions { CancellationToken = token },
            matrix => matrix.Rotate(degrees));
    }

    public void RotateMatrices2(IEnumerable<Matrix> matrices, float degrees,
        CancellationToken token)
    {
        // Warning: not recommended; see below.
        Parallel.ForEach(matrices, matrix =>
        {
            matrix.Rotate(degrees);
            token.ThrowIfCancellationRequested();
        });
    }

    public IEnumerable<int> MultiplyBy2(IEnumerable<int> values,
        CancellationToken cancellationToken)
    {
        return values.AsParallel()
            .WithCancellation(cancellationToken)
            .Select(item => item * 2);
    }
}

public class P8
{
    public async Task<HttpResponseMessage> GetWithTimeoutAsync(HttpClient client,
        string url, CancellationToken cancellationToken)
    {
        using CancellationTokenSource cts = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(2));
        CancellationToken combinedToken = cts.Token;

        return await client.GetAsync(url, combinedToken);
    }
}

public class P9
{
    public async Task<PingReply> PingAsync(string hostNameOrAddress, CancellationToken cancellationToken)
    {
        using var ping = new Ping();
        Task<PingReply> task = ping.SendPingAsync(hostNameOrAddress);
        using CancellationTokenRegistration _ = cancellationToken.Register(() => ping.SendAsyncCancel());
        return await task;
    }
}