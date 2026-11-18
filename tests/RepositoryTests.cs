namespace proj_tests;

[TestFixture]
public class SimulationRepositoryTests
{
    private PostgresFixture _pg = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _pg = new PostgresFixture();
        await _pg.InitializeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() => await _pg.DisposeAsync();

    private static Lesson[] FixedLessons(bool ask)
        => Enum.GetValues<Subject>()
            .Select(s => new Lesson(s, new FakeRandomSource(), new FixedRule { Result = ask }))
            .ToArray();

    private static Func<string, IStrategy> StrategyFactory =>
        _ => FixedStrategy.AttendAll();

    [Test]
    public async Task SaveAndRestore_AtDayD_ProducesIdenticalState()
    {
        await using var db = _pg.CreateContext();
        var repo = new SimulationRepository(db);

        var lessons = FixedLessons(ask: true);
        var uni = new University(FixedStrategy.AttendAll(), semDays: 10, classes: lessons);
        var sessionId = await repo.CreateSessionAsync(FixedStrategy.AttendAll(), lessons, semesterDays: 10);

        for (int i = 0; i < 3; i++)
        {
            uni.RunOneDay();
            await repo.SaveDayAsync(sessionId, uni);
        }

        var restored = await repo.LoadAsync(sessionId, StrategyFactory);

        Assert.Multiple(() =>
        {
            Assert.That(restored.Day, Is.EqualTo(uni.Day));
            Assert.That(restored.Student.Satisfaction, Is.EqualTo(uni.Student.Satisfaction));
            Assert.That(restored.DroppedOut, Is.EqualTo(uni.DroppedOut));
            Assert.That(restored.Finished, Is.EqualTo(uni.Finished));
            Assert.That(restored.Classes.Length, Is.EqualTo(uni.Classes.Length));
        });

        int n = TestHelpers.SubjectCount;
        for (int d = 0; d < uni.Day; d++)
            for (int s = 0; s < n; s++)
            {
                Assert.That(restored.Student.DaysPresent[d, s], Is.EqualTo(uni.Student.DaysPresent[d, s]),
                    $"DaysPresent[{d},{s}]");
                Assert.That(restored.Student.DaysWasAsked[d, s], Is.EqualTo(uni.Student.DaysWasAsked[d, s]),
                    $"DaysWasAsked[{d},{s}]");
            }
    }

    [Test]
    public async Task ContinueAfterRestore_ProducesSameFinalState()
    {
        const int semesterDays = 5;

        var baseline = new University(FixedStrategy.AttendAll(), semesterDays, FixedLessons(true));
        while (baseline.RunOneDay()) { }

        await using var db = _pg.CreateContext();
        var repo = new SimulationRepository(db);
        var lessons = FixedLessons(true);
        var uni = new University(FixedStrategy.AttendAll(), semesterDays, lessons);
        var sessionId = await repo.CreateSessionAsync(FixedStrategy.AttendAll(), lessons, semesterDays);

        for (int i = 0; i < 2; i++)
        {
            uni.RunOneDay();
            await repo.SaveDayAsync(sessionId, uni);
        }

        var restored = await repo.LoadAsync(sessionId, StrategyFactory);
        while (restored.RunOneDay()) { }

        Assert.Multiple(() =>
        {
            Assert.That(restored.Day, Is.EqualTo(baseline.Day));
            Assert.That(restored.Finished, Is.EqualTo(baseline.Finished));
            Assert.That(restored.DroppedOut, Is.EqualTo(baseline.DroppedOut));
            Assert.That(restored.Student.Satisfaction, Is.EqualTo(baseline.Student.Satisfaction));
        });
    }

    [Test]
    public async Task LessonParams_AreRestored_Identically()
    {
        var lessons = new[]
        {
            new Lesson((Subject)0, new FakeRandomSource().WithInts(2, 1), new ABRule()),
            new Lesson((Subject)1, new FakeRandomSource().WithInts(4, 1), new ARule()),
            new Lesson((Subject)2, new FakeRandomSource(), new FixedRule { Result = true }),
            new Lesson((Subject)3, new FakeRandomSource(), new FixedRule { Result = false }),
            new Lesson((Subject)4, new FakeRandomSource(), new FixedRule { Result = true }),
            new Lesson((Subject)5, new FakeRandomSource(), new FixedRule { Result = true }),
        };

        await using var db = _pg.CreateContext();
        var repo = new SimulationRepository(db);
        var sessionId = await repo.CreateSessionAsync(FixedStrategy.AttendAll(), lessons, semesterDays: 10);

        var restored = await repo.LoadAsync(sessionId, StrategyFactory);

        Assert.Multiple(() =>
        {
            Assert.That(restored.Classes[0].rule, Is.TypeOf<ABRule>());
            Assert.That(restored.Classes[0].SubjectA, Is.EqualTo((Subject)2));
            Assert.That(restored.Classes[0].SubjectB, Is.EqualTo((Subject)4));

            Assert.That(restored.Classes[1].rule, Is.TypeOf<ARule>());
            Assert.That(restored.Classes[1].SubjectA, Is.EqualTo((Subject)4));

            Assert.That(restored.Classes[2].rule, Is.TypeOf<FixedRule>());
            Assert.That(((FixedRule)restored.Classes[2].rule).Result, Is.True);

            Assert.That(restored.Classes[3].rule, Is.TypeOf<FixedRule>());
            Assert.That(((FixedRule)restored.Classes[3].rule).Result, Is.False);
        });
    }

    [Test]
    public async Task DroppedOut_StateIsPersistedAndRestored()
    {
        await using var db = _pg.CreateContext();
        var repo = new SimulationRepository(db);

        var lessons = FixedLessons(true);
        var uni = new University(FixedStrategy.SkipAll(), semDays: 10, classes: lessons);
        var sessionId = await repo.CreateSessionAsync(FixedStrategy.SkipAll(), lessons, semesterDays: 10);

        Assert.That(uni.RunOneDay(), Is.False);
        await repo.SaveDayAsync(sessionId, uni);

        var restored = await repo.LoadAsync(sessionId, _ => FixedStrategy.SkipAll());

        Assert.Multiple(() =>
        {
            Assert.That(restored.DroppedOut, Is.True);
            Assert.That(restored.Student.Satisfaction, Is.EqualTo(0f));
            Assert.That(restored.Day, Is.EqualTo(uni.Day));
            Assert.That(restored.RunOneDay(), Is.False);
        });
    }

    [Test]
    public async Task FindUnfinished_ReturnsLatestUnfinished()
    {
        await using var db = _pg.CreateContext();
        var repo = new SimulationRepository(db);

        var s1 = await repo.CreateSessionAsync(FixedStrategy.AttendAll(), FixedLessons(true), 3);
        var s2 = await repo.CreateSessionAsync(FixedStrategy.AttendAll(), FixedLessons(true), 3);

        var uni2 = await repo.LoadAsync(s2, StrategyFactory);
        bool alive;
        do
        {
            alive = uni2.RunOneDay();
            await repo.SaveDayAsync(s2, uni2);
        } while (alive);

        var unfinished = await repo.FindUnfinishedAsync();
        Assert.That(unfinished, Is.EqualTo(s1));
    }
}