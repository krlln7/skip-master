namespace proj_tests;

[TestFixture]
public class RuleTests
{
    private static IReadOnlyStudentHistory EmptyHistory
        => new Student(FixedStrategy.AttendAll());

    [Test]
    public void FixedRule_True_AlwaysAsks()
    {
        var rule = new FixedRule { Result = true };
        var lesson = TestHelpers.CreateLesson((Subject)0, rule);
        var history = EmptyHistory;

        for (int day = 0; day < 10; day++)
            Assert.That(rule.Decide(day, history, lesson), Is.True);
    }

    [Test]
    public void FixedRule_False_NeverAsks()
    {
        var rule = new FixedRule { Result = false };
        var lesson = TestHelpers.CreateLesson((Subject)0, rule);
        var history = EmptyHistory;

        for (int day = 0; day < 10; day++)
            Assert.That(rule.Decide(day, history, lesson), Is.False);
    }

    [Test]
    public void SequenceRule_FollowsConfiguredSequence()
    {
        var rule = new SequenceRule(true, true, false);
        var lesson = TestHelpers.CreateLesson((Subject)0, rule);
        var history = EmptyHistory;

        var actual = Enumerable.Range(0, 9)
            .Select(d => rule.Decide(d, history, lesson))
            .ToArray();

        var expected = new[] { true, true, false, true, true, false, true, true, false };
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void FixedRule_IgnoresDayAndHistory()
    {
        var rule = new FixedRule { Result = true };
        var lesson = TestHelpers.CreateLesson((Subject)0, rule);

        var s1 = new Student(FixedStrategy.AttendAll());
        for (int d = 0; d < 3; d++)
            for (int s = 0; s < TestHelpers.SubjectCount; s++)
                s1.DaysWasAsked[d, s] = true;

        Assert.That(rule.Decide(0, s1, lesson), Is.True);
        Assert.That(rule.Decide(42, s1, lesson), Is.True);
    }
}