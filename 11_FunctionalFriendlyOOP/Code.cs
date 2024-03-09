using System.Diagnostics;
using Nito.AsyncEx;

namespace _11_FunctionalFriendlyOOP;

public class P1
{
    public interface IMyAsyncInterface
    {
        Task<int> CountBytesAsync(HttpClient client, string url);
    }

    public class MyAsyncClass : IMyAsyncInterface
    {
        public async Task<int> CountBytesAsync(HttpClient client, string url)
        {
            var bytes = await client.GetByteArrayAsync(url);
            return bytes.Length;
        }
    }

    public async Task UseMyInterfaceAsync(HttpClient client, IMyAsyncInterface service)
    {
        var result = await service.CountBytesAsync(client, "http://www.example.com");
        Trace.WriteLine(result);
    }

    public class MyAsyncClassStub : IMyAsyncInterface
    {
        public Task<int> CountBytesAsync(HttpClient client, string url)
        {
            return Task.FromResult(13);
        }
    }
}

public class P2A
{
    public class MyAsyncClass
    {
        public Task InitializeAsync() => Task.CompletedTask;
    }

    public async Task Test()
    {
        var instance = new MyAsyncClass();
        await instance.InitializeAsync();
    }
}

public class P2B
{
    public class MyAsyncClass
    {
        private MyAsyncClass()
        {
        }

        private async Task<MyAsyncClass> InitializeAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return this;
        }

        public static Task<MyAsyncClass> CreateAsync()
        {
            var result = new MyAsyncClass();
            return result.InitializeAsync();
        }
    }

    public async Task Test()
    {
        MyAsyncClass instance = await MyAsyncClass.CreateAsync();
    }
}

public class P2C
{
    public class MyAsyncClass
    {
        public MyAsyncClass()
        {
            InitializeAsync();
        }

        // BAD CODE!!
        private async void InitializeAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}

public class P3A
{
    public interface IMyFundamentalType
    {
    }

    public interface IMyComposedType
    {
    }

    /// <summary>
    /// Marks a type as requiring asynchronous initialization 
    /// and provides the result of that initialization.
    /// </summary>
    public interface IAsyncInitialization
    {
        /// <summary>
        /// The result of the asynchronous initialization of this instance.
        /// </summary>
        Task Initialization { get; }
    }

    public class MyFundamentalType : IMyFundamentalType, IAsyncInitialization
    {
        public MyFundamentalType()
        {
            Initialization = InitializeAsync();
        }

        public Task Initialization { get; private set; }

        private async Task InitializeAsync()
        {
            // Asynchronously initialize this instance.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }


    public async Task Test1()
    {
        IMyFundamentalType instance = new MyFundamentalType(); // UltimateDIFactory.Create<IMyFundamentalType>();
        var instanceAsyncInit = instance as IAsyncInitialization;
        if (instanceAsyncInit != null)
            await instanceAsyncInit.Initialization;
    }


    public class MyComposedType : IMyComposedType, IAsyncInitialization
    {
        private readonly IMyFundamentalType _fundamental;

        public MyComposedType(IMyFundamentalType fundamental)
        {
            _fundamental = fundamental;
            Initialization = InitializeAsync();
        }

        public Task Initialization { get; private set; }

        private async Task InitializeAsync()
        {
            // Asynchronously wait for the fundamental instance to initialize,
            //  if necessary.
            var fundamentalAsyncInit = _fundamental as IAsyncInitialization;
            if (fundamentalAsyncInit != null)
                await fundamentalAsyncInit.Initialization;

            // Do our own initialization (synchronous or asynchronous).
            // ...
        }
    }


    public static class AsyncInitialization
    {
        public static Task WhenAllInitializedAsync(params object[] instances)
        {
            return Task.WhenAll(instances
                .OfType<IAsyncInitialization>()
                .Select(x => x.Initialization));
        }
    }


    public class MyComposedType2 : IMyComposedType, IAsyncInitialization
    {
        private readonly IMyFundamentalType _fundamental, _anotherType, _yetAnother;

        public MyComposedType2(IMyFundamentalType fundamental)
        {
            _fundamental = fundamental;
            Initialization = InitializeAsync();
        }

        public Task Initialization { get; private set; }

        private async Task InitializeAsync()
        {
            // Asynchronously wait for all 3 instances to initialize, if necessary.
            await AsyncInitialization.WhenAllInitializedAsync(_fundamental,
                _anotherType, _yetAnother);

            // Do our own initialization (synchronous or asynchronous).
            // ...
        }
    }
}

public class P4A
{
    // As an asynchronous method.
    public async Task<int> GetDataAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return 13;
    }
}

public class P4B
{
    // As a Task-returning property.
    // This API design is questionable.
    public Task<int> Data
    {
        get { return GetDataAsync(); }
    }

    private async Task<int> GetDataAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return 13;
    }
}

public class P4C
{
    // As a cached value.
    public AsyncLazy<int> Data
    {
        get { return _data; }
    }

    private readonly AsyncLazy<int> _data =
        new AsyncLazy<int>(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return 13;
        });

    public async Task Test()
    {
        var instance = new P4C();


        int value = await instance.Data;
    }
}


public class P4D
{
    private async Task<int> GetDataAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return 13;
    }

    public int Data
    {
        // BAD CODE!!
        get { return GetDataAsync().Result; }
    }
}

public class P5
{
    public class MyEventArgs : EventArgs, IDeferralSource
    {
        private readonly DeferralManager _deferrals = new DeferralManager();

        // ... // Your own constructors and properties.

        public IDisposable GetDeferral()
        {
            return _deferrals.DeferralSource.GetDeferral();
        }

        internal Task WaitForDeferralsAsync()
        {
            return _deferrals.WaitForDeferralsAsync();
        }
    }


    public event EventHandler<MyEventArgs> MyEvent;

    private async Task RaiseMyEventAsync()
    {
        EventHandler<MyEventArgs> handler = MyEvent;
        if (handler == null)
            return;

        var args = new MyEventArgs();
        handler(this, args);
        await args.WaitForDeferralsAsync();
    }


    public async void AsyncHandler(object sender, MyEventArgs args)
    {
        using IDisposable deferral = args.GetDeferral();
        await Task.Delay(TimeSpan.FromSeconds(2));
    }
}

public class P6A
{
    public class MyClass : IDisposable
    {
        private readonly CancellationTokenSource _disposeCts =
            new CancellationTokenSource();

        public async Task<int> CalculateValueAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2), _disposeCts.Token);
            return 13;
        }

        public void Dispose()
        {
            _disposeCts.Cancel();
        }
    }
}

public class P6B
{
    public class MyClass : IDisposable
    {
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();

        public async Task<int> CalculateValueAsync(CancellationToken cancellationToken)
        {
            using CancellationTokenSource combinedCts = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
            await Task.Delay(TimeSpan.FromSeconds(2), combinedCts.Token);
            return 13;
        }

        public void Dispose()
        {
            _disposeCts.Cancel();
        }
    }

    public async Task UseMyClassAsync()
    {
        Task<int> task;
        using (var resource = new MyClass())
        {
            task = resource.CalculateValueAsync(default);
        }

        // Throws OperationCanceledException.
        var result = await task;
    }
}

public class P6C
{
    public class MyClass : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }

    public async Task Test()
    {
        await using (var myClass = new MyClass())
        {
            // ...
        } // DisposeAsync is invoked (and awaited) here
    }

    public async Task Test2()
    {
        var myClass = new MyClass();
        await using (myClass.ConfigureAwait(false))
        {
            // ...
        } // DisposeAsync is invoked (and awaited) here with ConfigureAwait(false)
    }
}