class FifthyRule : IRule
{
    private readonly IRandomSource _random;

    public FifthyRule() : this(new TrueRandomSource()) { }
    public FifthyRule(IRandomSource random) => _random = random;

    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson lesson)
        => _random.NextDouble() > 0.5;
}