namespace proj_tests;

[TestFixture]
public class StrategyTests
{
    private static readonly bool[] AllTrue  = Enumerable.Repeat(true, 6).ToArray();
    private static readonly bool[] AllFalse = Enumerable.Repeat(false, 6).ToArray();

    [Test]
    public void FixedStrategy_AttendAll_ReturnsAllTrue()
    {
        var strategy = FixedStrategy.AttendAll();
        var result = strategy.DecideDay(0, new Student(strategy));
        Assert.That(result, Is.EqualTo(AllTrue));
    }

    [Test]
    public void FixedStrategy_SkipAll_ReturnsAllFalse()
    {
        var strategy = FixedStrategy.SkipAll();
        var result = strategy.DecideDay(0, new Student(strategy));
        Assert.That(result, Is.EqualTo(AllFalse));
    }

    [Test]
    public void FixedStrategy_Decision_IndependentOfDayAndHistory()
    {
        var strategy = FixedStrategy.AttendAll();
        var student = new Student(strategy);

        Assert.That(strategy.DecideDay(0, student), Is.EqualTo(strategy.DecideDay(5, student)));
    }

    [Test]
    public void PredictableStrategy_EmptyHistory_AttendsAll()
    {
        var strategy = new PredictableStrategy();
        var result = strategy.DecideDay(0, new Student(strategy));
        Assert.That(result, Is.All.False);
    }

    [Test]
    public void PredictableStrategy_ConsistentlyNotAsked_AttendsAll()
    {
        var strategy = new PredictableStrategy();
        var student = new Student(strategy);
        for (int d = 0; d < 3; d++)
            for (int s = 0; s < TestHelpers.SubjectCount; s++)
                student.DaysWasAsked[d, s] = false;

        Assert.That(strategy.DecideDay(3, student), Is.All.False);
    }

    [Test]
    public void PredictableStrategy_ConsistentlyAsked_SkipsAll()
    {
        var strategy = new PredictableStrategy();
        var student = new Student(strategy);
        for (int d = 0; d < 3; d++)
            for (int s = 0; s < TestHelpers.SubjectCount; s++)
                student.DaysWasAsked[d, s] = true;

        Assert.That(strategy.DecideDay(3, student), Is.All.True);
    }

    [Test]
    public void PredictableStrategy_MixedHistory_DecisionsAreIndependentPerSubject()
    {
        var strategy = new PredictableStrategy();
        var student = new Student(strategy);

        student.DaysWasAsked[0, 0] = true;
        student.DaysWasAsked[0, 1] = false;
        student.DaysWasAsked[0, 2] = true;
        student.DaysWasAsked[0, 3] = false;
        student.DaysWasAsked[0, 4] = true;
        student.DaysWasAsked[0, 5] = false;

        var result = strategy.DecideDay(1, student);
        Assert.That(result, Is.EqualTo(new[] { true, false, true, false, true, false }));
    }

    [Test]
    public void PredictableStrategy_OnlyPreviousDayMatters()
    {
        var strategy = new PredictableStrategy();
        var student = new Student(strategy);

        student.DaysWasAsked[0, 0] = true;
        student.DaysWasAsked[1, 0] = false;

        Assert.That(strategy.DecideDay(2, student)[0], Is.False);
    }
}