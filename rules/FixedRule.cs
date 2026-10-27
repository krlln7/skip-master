class FixedRule : IRule
{
    public bool Result { get; init; }
    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson lesson) => Result;
}