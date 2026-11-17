namespace proj_tests;

internal sealed class ScriptedStrategy : IStrategy
{
    private readonly bool[][] _perDay;
    public ScriptedStrategy(params bool[][] perDay) => _perDay = perDay;
    public string Name => "Scripted";
    public bool[] DecideDay(int day, IReadOnlyStudentHistory history)
        => (bool[])_perDay[day % _perDay.Length].Clone();
}