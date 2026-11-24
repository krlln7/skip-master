namespace proj_tests;

[TestFixture]
public class ABRuleTests
{
    private static Lesson CreateLessonWithAB(Subject a, Subject b)
    {

        int n = TestHelpers.SubjectCount;
        int offset = ((int)b - (int)a - 1 + n) % n;

        var rng = new FakeRandomSource().WithInts((int)a, offset);
        return new Lesson((Subject)0, rng, new ABRule());
    }

    [Test]
    public void Lesson_WithFakeRng_ProducesExpectedAAndB()
    {
        var lesson = CreateLessonWithAB((Subject)2, (Subject)5);
        Assert.That(lesson.SubjectA, Is.EqualTo((Subject)2));
        Assert.That(lesson.SubjectB, Is.EqualTo((Subject)5));
        Assert.That(lesson.SubjectA, Is.Not.EqualTo(lesson.SubjectB));
    }

    [Test]
    public void ABRule_IsDeterministic_AcrossIndependentInstances()
    {
        var lesson1 = CreateLessonWithAB((Subject)1, (Subject)3);
        var lesson2 = CreateLessonWithAB((Subject)1, (Subject)3);
        var student1 = new Student(FixedStrategy.AttendAll());
        var student2 = new Student(FixedStrategy.AttendAll());

        var seq1 = Enumerable.Range(0, 100)
            .Select(d => lesson1.rule.Decide(d, student1, lesson1))
            .ToArray();
        var seq2 = Enumerable.Range(0, 100)
            .Select(d => lesson2.rule.Decide(d, student2, lesson2))
            .ToArray();

        Assert.That(seq2, Is.EqualTo(seq1),
            "Same AB should give same decision sequence");
    }


    [TestCase(true, true, TestName = "ABRule independence: A=yes B=yes")]
    [TestCase(true, false, TestName = "ABRule independence: A=yes B=no")]
    [TestCase(false, true, TestName = "ABRule independence: A=no  B=yes")]
    [TestCase(false, false, TestName = "ABRule independence: A=no  B=no")]
    public void ABRule_DoesNotDependOnStudent_AllFourCombinations(bool aAsked, bool bAsked)
    {
        var lesson = CreateLessonWithAB((Subject)1, (Subject)4);

        var attend = new Student(FixedStrategy.AttendAll());
        var skip = new Student(FixedStrategy.SkipAll());

        foreach (var s in new[] { attend, skip })
        {
            s.DaysWasAsked[0, (int)lesson.SubjectA] = aAsked;
            s.DaysWasAsked[0, (int)lesson.SubjectB] = bAsked;
        }

        attend.DaysPresent[0, (int)lesson.SubjectA] = true;
        attend.DaysPresent[0, (int)lesson.SubjectB] = true;
        skip.DaysPresent[0, (int)lesson.SubjectA] = false;
        skip.DaysPresent[0, (int)lesson.SubjectB] = false;

        var r1 = lesson.rule.Decide(1, attend, lesson);
        var r2 = lesson.rule.Decide(1, skip, lesson);

        Assert.That(r2, Is.EqualTo(r1),
            $"A asked yesterday: {aAsked}, B asked yesterday: {bAsked} - must not depend on student");
    }
}