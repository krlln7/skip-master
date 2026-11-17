class FixedStrategy : IStrategy
{
    private readonly bool[] _skip;

    public string Name => "Fixed";

    public FixedStrategy(params bool[] skip) => _skip = skip;

    public bool[] DecideDay(int day, IReadOnlyStudentHistory history) => _skip;

    public static FixedStrategy SkipAll() => new([.. Enumerable.Repeat(false, Enum.GetValues<Subject>().Length)]);
    public static FixedStrategy AttendAll() => new([.. Enumerable.Repeat(true, Enum.GetValues<Subject>().Length)]);
}