interface IRule
{
    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson klass);
}