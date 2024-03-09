namespace _5_DataflowBasics;

using System.Threading.Tasks.Dataflow;

public class P1
{
    public void Test()
    {
        var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
        var subtractBlock = new TransformBlock<int, int>(item => item - 2);

        // After linking, values that exit multiplyBlock will enter subtractBlock.
        multiplyBlock.LinkTo(subtractBlock);
    }

    public async Task TestB()
    {
        var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
        var subtractBlock = new TransformBlock<int, int>(item => item - 2);

        var options = new DataflowLinkOptions { PropagateCompletion = true };
        multiplyBlock.LinkTo(subtractBlock, options);

        // ...

        // The first block's completion is automatically propagated to the second block.
        multiplyBlock.Complete();
        await subtractBlock.Completion;
    }
}

public class P2
{
    public void Test()
    {
        var block = new TransformBlock<int, int>(item =>
        {
            if (item == 1)
                throw new InvalidOperationException("Blech.");
            return item * 2;
        });
        block.Post(1);
        block.Post(2);
    }

    public async Task Test2()
    {
        try
        {
            var block = new TransformBlock<int, int>(item =>
            {
                if (item == 1)
                    throw new InvalidOperationException("Blech.");
                return item * 2;
            });
            block.Post(1);
            await block.Completion;
        }
        catch (InvalidOperationException)
        {
            // The exception is caught here.
        }
    }

    public async Task Test3()
    {
        try
        {
            var multiplyBlock = new TransformBlock<int, int>(item =>
            {
                if (item == 1)
                    throw new InvalidOperationException("Blech.");
                return item * 2;
            });
            var subtractBlock = new TransformBlock<int, int>(item => item - 2);
            multiplyBlock.LinkTo(subtractBlock,
                new DataflowLinkOptions { PropagateCompletion = true });
            multiplyBlock.Post(1);
            await subtractBlock.Completion;
        }
        catch (AggregateException)
        {
            // The exception is caught here.
        }
    }
}

public class P3
{
    public void Test()
    {
        var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
        var subtractBlock = new TransformBlock<int, int>(item => item - 2);

        IDisposable link = multiplyBlock.LinkTo(subtractBlock);
        multiplyBlock.Post(1);
        multiplyBlock.Post(2);

        // Unlink the blocks.
        // The data posted above may or may not have already gone through the link.
        // In real-world code, consider a using block rather than calling Dispose.
        link.Dispose();
    }
}

public class P4
{
    public void Test()
    {
        var sourceBlock = new BufferBlock<int>();
        var options = new DataflowBlockOptions { BoundedCapacity = 1 };
        var targetBlockA = new BufferBlock<int>(options);
        var targetBlockB = new BufferBlock<int>(options);

        sourceBlock.LinkTo(targetBlockA);
        sourceBlock.LinkTo(targetBlockB);
    }
}

public class P5
{
    public void Test()
    {
        var multiplyBlock = new TransformBlock<int, int>(
            item => item * 2,
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
            });
        var subtractBlock = new TransformBlock<int, int>(item => item - 2);
        multiplyBlock.LinkTo(subtractBlock);
    }
}

public class P6
{
    public IPropagatorBlock<int, int> CreateMyCustomBlock()
    {
        var multiplyBlock = new TransformBlock<int, int>(item => item * 2);
        var addBlock = new TransformBlock<int, int>(item => item + 2);
        var divideBlock = new TransformBlock<int, int>(item => item / 2);

        var flowCompletion = new DataflowLinkOptions { PropagateCompletion = true };
        multiplyBlock.LinkTo(addBlock, flowCompletion);
        addBlock.LinkTo(divideBlock, flowCompletion);

        return DataflowBlock.Encapsulate(multiplyBlock, divideBlock);
    }
}