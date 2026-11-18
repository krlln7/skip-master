namespace proj_tests;

[TestFixture]
public class ARuleTests
{
    private static Lesson CreateLessonWithA(Subject a)
    {
        int n = TestHelpers.SubjectCount;
        var rng = new FakeRandomSource().WithInts((int)a, 1);
        return new Lesson((Subject)0, rng, new ARule());
    }

    [Test]
    public void Lesson_WithFakeRng_ProducesExpectedA()
    {
        var lesson = CreateLessonWithA((Subject)3);
        Assert.That(lesson.SubjectA, Is.EqualTo((Subject)3));
    }

    [Test]
    public void ARule_IsDeterministic_AcrossIndependentInstances()
    {
        var lesson1 = CreateLessonWithA((Subject)2);
        var lesson2 = CreateLessonWithA((Subject)2);
        var student1 = new Student(FixedStrategy.AttendAll());
        var student2 = new Student(FixedStrategy.AttendAll());

        var seq1 = Enumerable.Range(0, 100)
            .Select(d => lesson1.rule.Decide(d, student1, lesson1))
            .ToArray();
        var seq2 = Enumerable.Range(0, 100)
            .Select(d => lesson2.rule.Decide(d, student2, lesson2))
            .ToArray();

        Assert.That(seq2, Is.EqualTo(seq1),
            "Same A should give same decision sequence");
    }

    [Test]
    public void ARule_DoesNotDependOnStudent()
    {
        var lesson = CreateLessonWithA((Subject)0);
        var attend = new Student(FixedStrategy.AttendAll());
        var skip = new Student(FixedStrategy.SkipAll());

        for (int day = 0; day < 100; day++)
            Assert.That(lesson.rule.Decide(day, attend, lesson),
                        Is.EqualTo(lesson.rule.Decide(day, skip, lesson)));
    }
}