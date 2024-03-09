using System.Threading.Tasks.Dataflow;
using Nito.AsyncEx;

namespace _7_Testing;

class Sut
{
    public Task<bool> MyMethodAsync() => Task.FromResult(true);

    public async void MyVoidMethodAsync()
    {
    }
}

class P1A
{
    [TestMethod]
    public async Task MyMethodAsync_ReturnsFalse()
    {
        var objectUnderTest = new Sut(); // ...;
        bool result = await objectUnderTest.MyMethodAsync();
        Assert.IsFalse(result);
    }
}

class P1B
{
    [TestMethod]
    public void MyMethodAsync_ReturnsFalse()
    {
        AsyncContext.Run(async () =>
        {
            var objectUnderTest = new Sut(); // ...;
            bool result = await objectUnderTest.MyMethodAsync();
            Assert.IsFalse(result);
        });
    }
}

class P1C
{
    interface IMyInterface
    {
        Task<int> SomethingAsync();
    }

    class SynchronousSuccess : IMyInterface
    {
        public Task<int> SomethingAsync()
        {
            return Task.FromResult(13);
        }
    }

    class SynchronousError : IMyInterface
    {
        public Task<int> SomethingAsync()
        {
            return Task.FromException<int>(new InvalidOperationException());
        }
    }

    class AsynchronousSuccess : IMyInterface
    {
        public async Task<int> SomethingAsync()
        {
            await Task.Yield(); // force asynchronous behavior
            return 13;
        }
    }
}

class P3
{
    // Not a recommended solution; see the rest of this section.
    [TestMethod]
    public void MyMethodAsync_DoesNotThrow()
    {
        AsyncContext.Run(() =>
        {
            var objectUnderTest = new Sut(); // ...;
            objectUnderTest.MyVoidMethodAsync();
        });
    }
}

class P4A
{
    static TransformBlock<int, int> CreateMyCustomBlock() => new TransformBlock<int, int>(x => x);

    [TestMethod]
    public async Task MyCustomBlock_AddsOneToDataItems()
    {
        var myCustomBlock = CreateMyCustomBlock();

        myCustomBlock.Post(3);
        myCustomBlock.Post(13);
        myCustomBlock.Complete();

        Assert.Equals(4, myCustomBlock.Receive());
        Assert.Equals(14, myCustomBlock.Receive());
        await myCustomBlock.Completion;
    }
}

class P4B
{
    static TransformBlock<int, int> CreateMyCustomBlock() => new TransformBlock<int, int>(x => x);

    [TestMethod]
    public async Task MyCustomBlock_Fault_DiscardsDataAndFaults()
    {
        var myCustomBlock = CreateMyCustomBlock();

        myCustomBlock.Post(3);
        myCustomBlock.Post(13);
        (myCustomBlock as IDataflowBlock).Fault(new InvalidOperationException());

        try
        {
            await myCustomBlock.Completion;
        }
        catch (AggregateException ex)
        {
            AssertExceptionIs<InvalidOperationException>(
                ex.Flatten().InnerException, false);
        }
    }

    public static void AssertExceptionIs<TException>(Exception ex,
        bool allowDerivedTypes = true)
    {
        if (allowDerivedTypes && !(ex is TException))
            Assert.Fail($"Exception is of type {ex.GetType().Name}, but " +
                        $"{typeof(TException).Name} or a derived type was expected.");
        if (!allowDerivedTypes && ex.GetType() != typeof(TException))
            Assert.Fail($"Exception is of type {ex.GetType().Name}, but " +
                        $"{typeof(TException).Name} was expected.");
    }
}