namespace proj_tests;

[TestFixture]
public class UniversityRunOneDayTests
{
    [Test]
    public void RunOneDay_AdvancesDay()
    {
        var lessons = TestHelpers.CreateLessons(new FixedRule { Result = false });
        var uni = new University(FixedStrategy.AttendAll(), semDays: 10, classes: lessons);

        Assert.That(uni.Day, Is.EqualTo(0));
        Assert.That(uni.RunOneDay(), Is.True);
        Assert.That(uni.Day, Is.EqualTo(1));
    }

    [Test]
    public void RunOneDay_Finishes_WhenSemesterEnds()
    {
        var lessons = TestHelpers.CreateLessons(new FixedRule { Result = false });
        var uni = new University(FixedStrategy.AttendAll(), semDays: 2, classes: lessons);

        Assert.That(uni.RunOneDay(), Is.True);
        Assert.That(uni.RunOneDay(), Is.False);
        Assert.That(uni.Finished, Is.True);
    }

    [Test]
    public void RunOneDay_AfterFinished_DoesNotAdvanceDay()
    {
        var lessons = TestHelpers.CreateLessons(new FixedRule { Result = false });
        var uni = new University(FixedStrategy.AttendAll(), semDays: 2, classes: lessons);

        uni.RunOneDay();
        uni.RunOneDay();

        Assert.That(uni.RunOneDay(), Is.False);
        Assert.That(uni.Day, Is.EqualTo(2));
    }

    [Test]
    public void RunOneDay_AfterDropout_ReturnsFalse_AndStops()
    {
        var lessons = TestHelpers.CreateLessons(new FixedRule { Result = true });
        var uni = new University(FixedStrategy.SkipAll(), semDays: 10, classes: lessons);

        Assert.That(uni.RunOneDay(), Is.False);
        Assert.That(uni.DroppedOut, Is.True);
        Assert.That(uni.Day, Is.EqualTo(0));
    }

    [Test]
    public void RunOneDay_AfterDropout_FurtherDaysNotProcessed()
    {
        var lessons = TestHelpers.CreateLessons(new FixedRule { Result = true });
        var uni = new University(FixedStrategy.SkipAll(), semDays: 10, classes: lessons);

        uni.RunOneDay();

        float satAfterDropout = uni.Student.Satisfaction;
        int dayAfterDropout = uni.Day;

        for (int i = 0; i < 5; i++)
            Assert.That(uni.RunOneDay(), Is.False);

        Assert.That(uni.Day, Is.EqualTo(dayAfterDropout));
        Assert.That(uni.Student.Satisfaction, Is.EqualTo(satAfterDropout));
        for (int d = 1; d < 10; d++)
            for (int s = 0; s < TestHelpers.SubjectCount; s++)
                Assert.That(uni.Student.DaysWasAsked[d, s], Is.Null);
    }

    [Test]
    public void RunOneDay_YesterdayState_IsPreserved()
    {
        var lessons = TestHelpers.CreateLessonsWithOverride(
            new FixedRule { Result = false },
            (Subject)0,
            new SequenceRule(true, false));

        var uni = new University(FixedStrategy.AttendAll(), semDays: 5, classes: lessons);

        uni.RunOneDay();
        Assert.That(uni.Student.DaysWasAsked[0, 0], Is.True);

        uni.RunOneDay();
        Assert.That(uni.Student.DaysWasAsked[1, 0], Is.False);
        Assert.That(uni.Student.DaysWasAsked[0, 0], Is.True);
    }
}