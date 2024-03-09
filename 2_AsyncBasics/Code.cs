using System.Diagnostics;
using System.Windows.Input;

namespace _2_AsyncBasics;

public class P1
{
    public async Task<string> DownloadStringWithRetries(HttpClient client, string uri)
    {
        var nextDelay = TimeSpan.FromSeconds(1);

        for (int i = 0; i != 3; ++i)
        {
            try
            {
                return await client.GetStringAsync(uri);
            }
            catch
            {
            }

            await Task.Delay(nextDelay);
            nextDelay += nextDelay;
        }

        return await client.GetStringAsync(uri);
    }

    public async Task<string?> DownloadStringWithTimeout(HttpClient client, string uri)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var downloadTask = client.GetStringAsync(uri);
        var timeoutTask = Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);

        var completedTask = await Task.WhenAny(downloadTask, timeoutTask);
        if (completedTask == timeoutTask)
            return null;

        return await downloadTask;
    }
}

public class P2A
{
    public interface IMyAsyncInterface
    {
        Task<int> GetValueAsync();
    }

    public class MySynchronousImplementation : IMyAsyncInterface
    {
        public Task<int> GetValueAsync()
        {
            return Task.FromResult(13);
        }
    }
}

public class P2B
{
    public interface IMyAsyncInterface
    {
        Task DoSomethingAsync();
    }

    public class MySynchronousImplementation : IMyAsyncInterface
    {
        public Task DoSomethingAsync()
        {
            return Task.CompletedTask;
        }
    }
}

public class P3
{
    public async Task MyMethodAsync(IProgress<double> progress = null)
    {
        bool done = false;
        double percentComplete = 0;
        while (!done)
        {
            // ...
            progress?.Report(percentComplete);
        }
    }

    public async Task CallMyMethodAsync()
    {
        var progress = new Progress<double>();
        progress.ProgressChanged += (sender, args) =>
        {
            // ...
        };
        await MyMethodAsync(progress);
    }
}

public class P4
{
    public async Task Test()
    {
        Task task1 = Task.Delay(TimeSpan.FromSeconds(1));
        Task task2 = Task.Delay(TimeSpan.FromSeconds(2));
        Task task3 = Task.Delay(TimeSpan.FromSeconds(1));

        await Task.WhenAll(task1, task2, task3);
    }

    public async Task Test2()
    {
        Task<int> task1 = Task.FromResult(3);
        Task<int> task2 = Task.FromResult(5);
        Task<int> task3 = Task.FromResult(7);

        int[] results = await Task.WhenAll(task1, task2, task3);

        // "results" contains { 3, 5, 7 }
    }

    public async Task<string> DownloadAllAsync(HttpClient client, IEnumerable<string> urls)
    {
        // Define the action to do for each URL.
        var downloads = urls.Select(url => client.GetStringAsync(url));
        // Note that no tasks have actually started yet
        //  because the sequence is not evaluated.

        // Start all URLs downloading simultaneously.
        Task<string>[] downloadTasks = downloads.ToArray();
        // Now the tasks have all started.

        // Asynchronously wait for all downloads to complete.
        string[] htmlPages = await Task.WhenAll(downloadTasks);

        return string.Concat(htmlPages);
    }


    public async Task ThrowNotImplementedExceptionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task ThrowInvalidOperationExceptionAsync()
    {
        throw new InvalidOperationException();
    }

    public async Task ObserveOneExceptionAsync()
    {
        var task1 = ThrowNotImplementedExceptionAsync();
        var task2 = ThrowInvalidOperationExceptionAsync();

        try
        {
            await Task.WhenAll(task1, task2);
        }
        catch (Exception ex)
        {
            // "ex" is either NotImplementedException or InvalidOperationException.
            // ...
        }
    }

    public async Task ObserveAllExceptionsAsync()
    {
        var task1 = ThrowNotImplementedExceptionAsync();
        var task2 = ThrowInvalidOperationExceptionAsync();

        Task allTasks = Task.WhenAll(task1, task2);
        try
        {
            await allTasks;
        }
        catch
        {
            AggregateException allExceptions = allTasks.Exception;
            // ...
        }
    }
}

public class P5
{
    // Returns the length of data at the first URL to respond.
    public async Task<int> FirstRespondingUrlAsync(HttpClient client, string urlA, string urlB)
    {
        // Start both downloads concurrently.
        Task<byte[]> downloadTaskA = client.GetByteArrayAsync(urlA);
        Task<byte[]> downloadTaskB = client.GetByteArrayAsync(urlB);

        // Wait for either of the tasks to complete.
        Task<byte[]> completedTask =
            await Task.WhenAny(downloadTaskA, downloadTaskB);

        // Return the length of the data retrieved from that URL.
        byte[] data = await completedTask;
        return data.Length;
    }
}

public class P6A
{
    public async Task<int> DelayAndReturnAsync(int value)
    {
        await Task.Delay(TimeSpan.FromSeconds(value));
        return value;
    }

    // Currently, this method prints "2", "3", and "1".
    // The desired behavior is for this method to print "1", "2", and "3".
    public async Task ProcessTasksAsync()
    {
        // Create a sequence of tasks.
        Task<int> taskA = DelayAndReturnAsync(2);
        Task<int> taskB = DelayAndReturnAsync(3);
        Task<int> taskC = DelayAndReturnAsync(1);
        Task<int>[] tasks = new[] { taskA, taskB, taskC };

        // Await each task in order.
        foreach (Task<int> task in tasks)
        {
            var result = await task;
            Trace.WriteLine(result);
        }
    }
}

public class P6B
{
    public async Task<int> DelayAndReturnAsync(int value)
    {
        await Task.Delay(TimeSpan.FromSeconds(value));
        return value;
    }

    public async Task AwaitAndProcessAsync(Task<int> task)
    {
        int result = await task;
        Trace.WriteLine(result);
    }

    // This method now prints "1", "2", and "3".
    public async Task ProcessTasksAsync()
    {
        // Create a sequence of tasks.
        Task<int> taskA = DelayAndReturnAsync(2);
        Task<int> taskB = DelayAndReturnAsync(3);
        Task<int> taskC = DelayAndReturnAsync(1);
        Task<int>[] tasks = new[] { taskA, taskB, taskC };

        IEnumerable<Task> taskQuery =
            from t in tasks select AwaitAndProcessAsync(t);
        Task[] processingTasks = taskQuery.ToArray();

        // Await all processing to complete
        await Task.WhenAll(processingTasks);
    }
}

public class P6C
{
    public async Task<int> DelayAndReturnAsync(int value)
    {
        await Task.Delay(TimeSpan.FromSeconds(value));
        return value;
    }

    // This method now prints "1", "2", and "3".
    public async Task ProcessTasksAsync()
    {
        // Create a sequence of tasks.
        Task<int> taskA = DelayAndReturnAsync(2);
        Task<int> taskB = DelayAndReturnAsync(3);
        Task<int> taskC = DelayAndReturnAsync(1);
        Task<int>[] tasks = new[] { taskA, taskB, taskC };

        Task[] processingTasks = tasks.Select(async t =>
        {
            var result = await t;
            Trace.WriteLine(result);
        }).ToArray();

        // Await all processing to complete
        await Task.WhenAll(processingTasks);
    }
}

public class P7
{
    public async Task ResumeOnContextAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));

        // This method resumes within the same context.
    }

    public async Task ResumeWithoutContextAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        // This method discards its context when it resumes.
    }
}

public class P8A
{
    public async Task ThrowExceptionAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        throw new InvalidOperationException("Test");
    }

    public async Task TestAsync()
    {
        try
        {
            await ThrowExceptionAsync();
        }
        catch (InvalidOperationException)
        {
        }
    }
}

public class P8B
{
    public async Task ThrowExceptionAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        throw new InvalidOperationException("Test");
    }

    public async Task TestAsync()
    {
        // The exception is thrown by the method and placed on the task.
        Task task = ThrowExceptionAsync();
        try
        {
            // The exception is re-raised here, where the task is awaited.
            await task;
        }
        catch (InvalidOperationException)
        {
            // The exception is correctly caught here.
        }
    }
}

public class P9A
{
    public sealed class MyAsyncCommand : ICommand
    {
        async void ICommand.Execute(object parameter)
        {
            await Execute(parameter);
        }

        public async Task Execute(object? parameter)
        {
            // ... // Asynchronous command implementation goes here.
        }

        // ... // Other members (CanExecute, etc)
        public bool CanExecute(object parameter)
        {
            CanExecuteChanged?.Invoke(null, null);
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
    }
}

public class P10A
{
    public async ValueTask<int> MethodAsync()
    {
        await Task.Delay(100); // asynchronous work.
        return 13;
    }
}

public class P10C
{
    private Func<Task> _disposeLogic;

    public ValueTask DisposeAsync()
    {
        if (_disposeLogic == null)
            return default;

        // Note: this simple example is not threadsafe;
        //  if multiple threads call DisposeAsync,
        //  the logic could run more than once.
        Func<Task> logic = _disposeLogic;
        _disposeLogic = null;
        return new ValueTask(logic());
    }
}

public class P11A
{
    ValueTask<int> MethodAsync() => new ValueTask<int>(13);

    public async Task ConsumingMethodAsync()
    {
        int value = await MethodAsync();
    }
}

public class P11B
{
    ValueTask<int> MethodAsync() => new ValueTask<int>(13);

    public async Task ConsumingMethodAsync()
    {
        ValueTask<int> valueTask = MethodAsync();
        // ... // other concurrent work
        int value = await valueTask;
    }
}

public class P11C
{
    ValueTask<int> MethodAsync() => new ValueTask<int>(13);

    public async Task ConsumingMethodAsync()
    {
        Task<int> task = MethodAsync().AsTask();
        // ... // other concurrent work
        int value = await task;
        int anotherValue = await task;
    }
}

public class P11D
{
    ValueTask<int> MethodAsync() => new ValueTask<int>(13);

    public async Task ConsumingMethodAsync()
    {
        Task<int> task1 = MethodAsync().AsTask();
        Task<int> task2 = MethodAsync().AsTask();
        int[] results = await Task.WhenAll(task1, task2);
    }
}