class FifthyRule : IRule
{
    public bool Decide(int day, IReadOnlyStudentHistory history, Lesson klass)
    {
        double decision = Random.Shared.NextDouble();
        return (decision > 0.5f) ? true : false;
    }
}