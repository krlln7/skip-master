class ABRule : IRule
{
    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson klass)
    {
        if (day == 0) return false;

        bool askedA = history.WasAsked(day - 1, klass.SubjectA).GetValueOrDefault(false);
        bool askedB = history.WasAsked(day - 1, klass.SubjectB).GetValueOrDefault(false);

        if (askedA && !askedB)
        {
            return true;
        }
        else if (askedB && !askedA)
        {
            return true;
        }
        return false;
    }
}