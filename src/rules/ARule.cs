class ARule : IRule
{
    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson klass)
    {
        if (day == 0) return false;
        return history.WasAsked(day - 1, klass.SubjectA) == true ? true : false;
    }
}