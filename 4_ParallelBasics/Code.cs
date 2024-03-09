using System.Diagnostics;

namespace _4_ParallelBasics;

public class P1A
{
    public abstract class Matrix
    {
        public abstract void Rotate(float degrees);
    }

    public void RotateMatrices(IEnumerable<Matrix> matrices, float degrees)
    {
        Parallel.ForEach(matrices, matrix => matrix.Rotate(degrees));
    }
}

public class P1B
{
    public abstract class Matrix
    {
        public bool IsInvertible => true;
        public abstract void Invert();
    }

    public void InvertMatrices(IEnumerable<Matrix> matrices)
    {
        Parallel.ForEach(matrices, (matrix, state) =>
        {
            if (!matrix.IsInvertible)
                state.Stop();
            else
                matrix.Invert();
        });
    }
}

public class P1C
{
    public abstract class Matrix
    {
        public abstract void Rotate(float degrees);
    }

    public void RotateMatrices(IEnumerable<Matrix> matrices, float degrees, CancellationToken token)
    {
        Parallel.ForEach(matrices,
            new ParallelOptions { CancellationToken = token },
            matrix => matrix.Rotate(degrees));
    }
}

public class P1D
{
    public abstract class Matrix
    {
        public bool IsInvertible => true;
        public abstract void Invert();
    }

    // Note: this is not the most efficient implementation.
    // This is just an example of using a lock to protect shared state.
    public int InvertMatrices(IEnumerable<Matrix> matrices)
    {
        object mutex = new object();
        int nonInvertibleCount = 0;
        Parallel.ForEach(matrices, matrix =>
        {
            if (matrix.IsInvertible)
            {
                matrix.Invert();
            }
            else
            {
                lock (mutex)
                {
                    ++nonInvertibleCount;
                }
            }
        });
        return nonInvertibleCount;
    }
}

public class P2A
{
    // Note: this is not the most efficient implementation.
    // This is just an example of using a lock to protect shared state.
    public int ParallelSum(IEnumerable<int> values)
    {
        object mutex = new object();
        int result = 0;
        Parallel.ForEach(source: values,
            localInit: () => 0,
            body: (item, state, localValue) => localValue + item,
            localFinally: localValue =>
            {
                lock (mutex)
                    result += localValue;
            });
        return result;
    }
}

public class P2B
{
    public int ParallelSum(IEnumerable<int> values)
    {
        return values.AsParallel().Sum();
    }
}

public class P2C
{
    public int ParallelSum(IEnumerable<int> values)
    {
        return values.AsParallel().Aggregate(
            seed: 0,
            func: (sum, item) => sum + item
        );
    }
}

public class P3
{
    public void ProcessArray(double[] array)
    {
        Parallel.Invoke(
            () => ProcessPartialArray(array, 0, array.Length / 2),
            () => ProcessPartialArray(array, array.Length / 2, array.Length)
        );
    }

    public void ProcessPartialArray(double[] array, int begin, int end)
    {
        // CPU-intensive processing...
    }

    public void DoAction20Times(Action action)
    {
        Action[] actions = Enumerable.Repeat(action, 20).ToArray();
        Parallel.Invoke(actions);
    }

    public void DoAction20Times(Action action, CancellationToken token)
    {
        Action[] actions = Enumerable.Repeat(action, 20).ToArray();
        Parallel.Invoke(new ParallelOptions { CancellationToken = token }, actions);
    }
}

public class P4A
{
    public class Node
    {
        public Node Left { get; }
        public Node Right { get; }
    }

    public void DoExpensiveActionOnNode(Node node)
    {
    }

    public void Traverse(Node current)
    {
        DoExpensiveActionOnNode(current);
        if (current.Left != null)
        {
            Task.Factory.StartNew(
                () => Traverse(current.Left),
                CancellationToken.None,
                TaskCreationOptions.AttachedToParent,
                TaskScheduler.Default);
        }

        if (current.Right != null)
        {
            Task.Factory.StartNew(
                () => Traverse(current.Right),
                CancellationToken.None,
                TaskCreationOptions.AttachedToParent,
                TaskScheduler.Default);
        }
    }

    public void ProcessTree(Node root)
    {
        Task task = Task.Factory.StartNew(
            () => Traverse(root),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default);
        task.Wait();
    }
}

public class P4B
{
    public void Test()
    {
        Task task = Task.Factory.StartNew(
            () => Thread.Sleep(TimeSpan.FromSeconds(2)),
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default);
        Task continuation = task.ContinueWith(
            t => Trace.WriteLine("Task is done"),
            CancellationToken.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);
        // The "t" argument to the continuation is the same as "task".
    }
}

public class P5A
{
    public IEnumerable<int> MultiplyBy2(IEnumerable<int> values)
    {
        return values.AsParallel().Select(value => value * 2);
    }
}

public class P5B
{
    public IEnumerable<int> MultiplyBy2(IEnumerable<int> values)
    {
        return values.AsParallel().AsOrdered().Select(value => value * 2);
    }

    public int ParallelSum(IEnumerable<int> values)
    {
        return values.AsParallel().Sum();
    }
}