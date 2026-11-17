namespace proj_tests;

internal sealed class SequenceRule : IRule
{
    private readonly bool[] _sequence;
    public SequenceRule(params bool[] sequence) => _sequence = sequence;
    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson lesson)
        => _sequence[day % _sequence.Length];
}