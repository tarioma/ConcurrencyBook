using System.Runtime.CompilerServices;

namespace _3_AsynchronousStreams;

public class P1
{
    public async IAsyncEnumerable<int> GetValuesAsync()
    {
        await Task.Delay(1000); // some asynchronous work
        yield return 10;
        await Task.Delay(1000); // more asynchronous work
        yield return 13;
    }

    public async IAsyncEnumerable<string> GetValuesAsync(HttpClient client)
    {
        int offset = 0;
        const int limit = 10;
        while (true)
        {
            // Get the current page of results and parse them
            string result = await client.GetStringAsync(
                $"https://example.com/api/values?offset={offset}&limit={limit}");
            string[] valuesOnThisPage = result.Split('\n');

            // Produce the results for this page
            foreach (string value in valuesOnThisPage)
                yield return value;

            // If this is the last page, we're done
            if (valuesOnThisPage.Length != limit)
                break;

            // Otherwise, proceed to the next page
            offset += limit;
        }
    }
}

public abstract class P2A
{
    public abstract IAsyncEnumerable<string> GetValuesAsync(HttpClient client);

    public async Task ProcessValueAsync(HttpClient client)
    {
        await foreach (string value in GetValuesAsync(client))
        {
            Console.WriteLine(value);
        }
    }
}

public abstract class P2B
{
    public abstract IAsyncEnumerable<string> GetValuesAsync(HttpClient client);

    public async Task ProcessValueAsync(HttpClient client)
    {
        await foreach (string value in GetValuesAsync(client))
        {
            await Task.Delay(100); // asynchronous work
            Console.WriteLine(value);
        }
    }
}

public abstract class P2C
{
    public abstract IAsyncEnumerable<string> GetValuesAsync(HttpClient client);

    public async Task ProcessValueAsync(HttpClient client)
    {
        await foreach (string value in GetValuesAsync(client).ConfigureAwait(false))
        {
            await Task.Delay(100).ConfigureAwait(false); // asynchronous work
            Console.WriteLine(value);
        }
    }
}

public class P4A
{
    public async Task Test()
    {
        await foreach (int result in SlowRange())
        {
            Console.WriteLine(result);
            if (result >= 8)
                break;
        }

        // Produce sequence that slows down as it progresses
        async IAsyncEnumerable<int> SlowRange()
        {
            for (int i = 0; i != 10; ++i)
            {
                await Task.Delay(i * 100);
                yield return i;
            }
        }
    }
}

public class P4B
{
    public async Task Test2()
    {
        using var cts = new CancellationTokenSource(500);
        CancellationToken token = cts.Token;
        await foreach (int result in SlowRange(token))
        {
            Console.WriteLine(result);
        }
    }

    // Produce sequence that slows down as it progresses
    public async IAsyncEnumerable<int> SlowRange([EnumeratorCancellation] CancellationToken token = default)
    {
        for (int i = 0; i != 10; ++i)
        {
            await Task.Delay(i * 100, token);
            yield return i;
        }
    }
}

public class P4C
{
    public async Task ConsumeSequence(IAsyncEnumerable<int> items)
    {
        using var cts = new CancellationTokenSource(500);
        CancellationToken token = cts.Token;
        await foreach (int result in items.WithCancellation(token))
        {
            Console.WriteLine(result);
        }
    }

    // Produce sequence that slows down as it progresses
    public async IAsyncEnumerable<int> SlowRange(
        [EnumeratorCancellation] CancellationToken token = default)
    {
        for (int i = 0; i != 10; ++i)
        {
            await Task.Delay(i * 100, token);
            yield return i;
        }
    }

    public async Task Test() => await ConsumeSequence(SlowRange());
}

public class P4D
{
    public async Task ConsumeSequence(IAsyncEnumerable<int> items)
    {
        using var cts = new CancellationTokenSource(500);
        CancellationToken token = cts.Token;
        await foreach (int result in items.WithCancellation(token).ConfigureAwait(false))
        {
            Console.WriteLine(result);
        }
    }
}