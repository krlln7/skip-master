public interface IRandomSource
{
    double NextDouble();
    int Next(int low, int high);
}

public sealed class TrueRandomSource : IRandomSource
{
    public double NextDouble() => Random.Shared.NextDouble();
    public int Next(int low, int high) => Random.Shared.Next(low, high);
}

public sealed class FakeRandomSource : IRandomSource
{
    private readonly Queue<double> _doubles = new();
    private readonly Queue<int> _ints = new();

    public FakeRandomSource WithDoubles(params double[] v) { foreach (var x in v) _doubles.Enqueue(x); return this; }
    public FakeRandomSource WithInts(params int[] v) { foreach (var x in v) _ints.Enqueue(x); return this; }

    public double NextDouble() => _doubles.Count > 0 ? _doubles.Dequeue()
        : throw new InvalidOperationException("no doubles left");
    public int Next(int low, int high) => _ints.Count > 0 ? _ints.Dequeue()
        : low + (int)(NextDouble() * (high - low));
}