namespace _13_Scheduling;

public class P1
{
    public void Test()
    {
        Task task = Task.Run(() => { Thread.Sleep(TimeSpan.FromSeconds(2)); });
    }

    public void Test2()
    {
        Task<int> task = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return 13;
        });
    }
}

public class P2
{
    public void Test()
    {
        TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
    }

    public void Test2()
    {
        var schedulerPair = new ConcurrentExclusiveSchedulerPair();
        TaskScheduler concurrent = schedulerPair.ConcurrentScheduler;
        TaskScheduler exclusive = schedulerPair.ExclusiveScheduler;
    }

    public void Test3()
    {
        var schedulerPair = new ConcurrentExclusiveSchedulerPair(
            TaskScheduler.Default, maxConcurrencyLevel: 8);
        TaskScheduler scheduler = schedulerPair.ConcurrentScheduler;
    }
}

public class P3
{
    public abstract class Matrix
    {
        public abstract void Rotate(float degrees);
    }

    public void RotateMatrices(IEnumerable<IEnumerable<Matrix>> collections, float degrees)
    {
        var schedulerPair = new ConcurrentExclusiveSchedulerPair(
            TaskScheduler.Default, maxConcurrencyLevel: 8);
        TaskScheduler scheduler = schedulerPair.ConcurrentScheduler;
        ParallelOptions options = new ParallelOptions { TaskScheduler = scheduler };
        Parallel.ForEach(collections, options,
            matrices => Parallel.ForEach(matrices, options,
                matrix => matrix.Rotate(degrees)));
    }
}