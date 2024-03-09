using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;

namespace _14_Scenarios;

public class P1A
{
    static int _simpleValue;
    static readonly Lazy<int> MySharedInteger = new Lazy<int>(() => _simpleValue++);

    public void UseSharedInteger()
    {
        int sharedValue = MySharedInteger.Value;
    }
}

public class P1B
{
    public static int _simpleValue;

    public static readonly Lazy<Task<int>> MySharedAsyncInteger =
        new Lazy<Task<int>>(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            return _simpleValue++;
        });

    public async Task GetSharedIntegerAsync()
    {
        int sharedValue = await MySharedAsyncInteger.Value;
    }
}

public class P1C
{
    public static int _simpleValue;

    public static readonly Lazy<Task<int>> MySharedAsyncInteger =
        new Lazy<Task<int>>(() => Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return _simpleValue++;
        }));

    public async Task GetSharedIntegerAsync()
    {
        int sharedValue = await MySharedAsyncInteger.Value;
    }
}

public class P1D
{
    public sealed class AsyncLazy<T>
    {
        private readonly object _mutex;
        private readonly Func<Task<T>> _factory;
        private Lazy<Task<T>> _instance;

        public AsyncLazy(Func<Task<T>> factory)
        {
            _mutex = new object();
            _factory = RetryOnFailure(factory);
            _instance = new Lazy<Task<T>>(_factory);
        }

        private Func<Task<T>> RetryOnFailure(Func<Task<T>> factory)
        {
            return async () =>
            {
                try
                {
                    return await factory().ConfigureAwait(false);
                }
                catch
                {
                    lock (_mutex)
                    {
                        _instance = new Lazy<Task<T>>(_factory);
                    }

                    throw;
                }
            };
        }

        public Task<T> Task
        {
            get
            {
                lock (_mutex)
                    return _instance.Value;
            }
        }
    }

    public static int _simpleValue;

    public static readonly AsyncLazy<int> MySharedAsyncInteger =
        new AsyncLazy<int>(() => Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return _simpleValue++;
        }));

    public async Task GetSharedIntegerAsync()
    {
        int sharedValue = await MySharedAsyncInteger.Task;
    }
}

public class P3B
{
    public class BindableTask<T> : INotifyPropertyChanged
    {
        private readonly Task<T> _task;

        public BindableTask(Task<T> task)
        {
            _task = task;
            var _ = WatchTaskAsync();
        }

        private async Task WatchTaskAsync()
        {
            try
            {
                await _task;
            }
            catch
            {
            }

            OnPropertyChanged("IsNotCompleted");
            OnPropertyChanged("IsSuccessfullyCompleted");
            OnPropertyChanged("IsFaulted");
            OnPropertyChanged("Result");
        }

        public bool IsNotCompleted
        {
            get { return !_task.IsCompleted; }
        }

        public bool IsSuccessfullyCompleted
        {
            get { return _task.Status == TaskStatus.RanToCompletion; }
        }

        public bool IsFaulted
        {
            get { return _task.IsFaulted; }
        }

        public T Result
        {
            get { return IsSuccessfullyCompleted ? _task.Result : default; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

public class P4A
{
    private static AsyncLocal<Guid> _operationId = new AsyncLocal<Guid>();

    public async Task DoLongOperationAsync()
    {
        _operationId.Value = Guid.NewGuid();

        await DoSomeStepOfOperationAsync();
    }

    public async Task DoSomeStepOfOperationAsync()
    {
        await Task.Delay(100); // some async work

        // Do some logging here.
        Trace.WriteLine("In operation: " + _operationId.Value);
    }
}

public class P4B
{
    internal sealed class AsyncLocalGuidStack
    {
        private readonly AsyncLocal<ImmutableStack<Guid>> _operationIds =
            new AsyncLocal<ImmutableStack<Guid>>();

        private ImmutableStack<Guid> Current =>
            _operationIds.Value ?? ImmutableStack<Guid>.Empty;

        public IDisposable Push(Guid value)
        {
            _operationIds.Value = Current.Push(value);
            return new PopWhenDisposed(this);
        }

        private void Pop()
        {
            ImmutableStack<Guid> newValue = Current.Pop();
            if (newValue.IsEmpty)
                newValue = null;
            _operationIds.Value = newValue;
        }

        public IEnumerable<Guid> Values => Current;

        private sealed class PopWhenDisposed : IDisposable
        {
            private AsyncLocalGuidStack _stack;

            public PopWhenDisposed(AsyncLocalGuidStack stack) =>
                _stack = stack;

            public void Dispose()
            {
                _stack?.Pop();
                _stack = null;
            }
        }
    }

    private static AsyncLocalGuidStack _operationIds = new AsyncLocalGuidStack();

    public async Task DoLongOperationAsync()
    {
        using (_operationIds.Push(Guid.NewGuid()))
            await DoSomeStepOfOperationAsync();
    }

    public async Task DoSomeStepOfOperationAsync()
    {
        await Task.Delay(100); // some async work

        // Do some logging here.
        Trace.WriteLine("In operation: " +
                        string.Join(":", _operationIds.Values));
    }
}

public class P5
{
    private async Task<int> DelayAndReturnCore(bool sync)
    {
        int value = 100;

        // Do some work
        if (sync)
            Thread.Sleep(value); // call synchronous API
        else
            await Task.Delay(value); // call asynchronous API

        return value;
    }

    // Asynchronous API
    public Task<int> DelayAndReturnAsync() =>
        DelayAndReturnCore(sync: false);

    // Synchronous API
    public int DelayAndReturn() =>
        DelayAndReturnCore(sync: true).GetAwaiter().GetResult();
}