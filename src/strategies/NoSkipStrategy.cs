class NoSkipStrategy : IStrategy
{
    public String Name => "NoSkip";
    public bool[] DecideDay(int day, IReadOnlyStudentHistory history)
    {
        bool[] ret = new bool[Enum.GetValues<Subject>().Length];
        Array.Fill(ret, true);
        return ret;
    }
}