class FifthyRule : IRule
{
    public int Seed { get; init; } = Random.Shared.Next();
    private readonly IRandomSource _random;

    public FifthyRule() : this(new TrueRandomSource()) { }
    public FifthyRule(IRandomSource random) => _random = random;

    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson lesson)
    {
        var r = new Random(StableCombine(Seed, day, (int)lesson.Subject));
        return r.NextDouble() > 0.5;
    }

    private static int StableCombine(int seed, int day, int subject)
    => unchecked(seed * 397 ^ day * 31 ^ subject);
}