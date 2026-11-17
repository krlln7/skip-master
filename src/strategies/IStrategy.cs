interface IStrategy
{
    string Name { get; }

    // skip or not to skip?
    public bool[] DecideDay(int day, IReadOnlyStudentHistory history);
}