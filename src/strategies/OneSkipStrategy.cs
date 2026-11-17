class OneSkipStrategy : IStrategy
{
    public String Name => "OneSkip";
    public bool[] DecideDay(int day, IReadOnlyStudentHistory history)
    {
        bool[] ret = new bool[Enum.GetValues<Subject>().Length];
        Array.Fill(ret, true);
        ret[0] = false;
        return ret;
    }
}