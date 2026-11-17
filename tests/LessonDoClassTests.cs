namespace proj_tests;

[TestFixture]
public class LessonDoClassTests
{
    [Test]
    public void Present_And_Asked_RecordsWasAsked_NoDropout()
    {
        var student = new Student(FixedStrategy.AttendAll());
        student.DoDay(0);
        var lesson = TestHelpers.CreateLesson((Subject)1, new FixedRule { Result = true });

        lesson.DoClass(0, student);

        Assert.That(student.DaysWasAsked[0, 1], Is.True);
        Assert.That(student.Satisfaction, Is.GreaterThan(0));
    }

    [Test]
    public void Present_And_NotAsked_RecordsWasAskedFalse_NoDropout()
    {
        var student = new Student(FixedStrategy.AttendAll());
        student.DoDay(0);
        var lesson = TestHelpers.CreateLesson((Subject)1, new FixedRule { Result = false });

        lesson.DoClass(0, student);

        Assert.That(student.DaysWasAsked[0, 1], Is.False);
        Assert.That(student.Satisfaction, Is.GreaterThan(0));
    }

    [Test]
    public void Absent_And_Asked_DropsOut_And_SetsLastSubject()
    {
        var subject = (Subject)3;
        var student = new Student(FixedStrategy.SkipAll());
        student.DoDay(0);
        var lesson = TestHelpers.CreateLesson(subject, new FixedRule { Result = true });

        lesson.DoClass(0, student);

        Assert.That(student.Satisfaction, Is.EqualTo(0));
        Assert.That(student.LastSubject, Is.EqualTo(subject));
        Assert.That(student.DaysWasAsked[0, (int)subject], Is.True);
    }

    [Test]
    public void Absent_And_NotAsked_NoDropout_WasAskedFalse()
    {
        var student = new Student(FixedStrategy.SkipAll());
        student.DoDay(0);
        var lesson = TestHelpers.CreateLesson((Subject)2, new FixedRule { Result = false });

        lesson.DoClass(0, student);

        Assert.That(student.DaysWasAsked[0, 2], Is.False);
        Assert.That(student.Satisfaction, Is.GreaterThan(0));
    }
}