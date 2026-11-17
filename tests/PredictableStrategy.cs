namespace proj_tests;

internal sealed class PredictableStrategy : IStrategy
{
    public string Name => "Predictable";

    public bool[] DecideDay(int day, IReadOnlyStudentHistory history)
    {
        int n = Enum.GetValues<Subject>().Length;
        var result = new bool[n];
        for (int s = 0; s < n; s++)
            result[s] = day > 0 && history.WasAsked(day - 1, (Subject)s) == true;
        return result;
    }
}