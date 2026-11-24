using Microsoft.EntityFrameworkCore;

class SimulationRepository
{
    private readonly UniversityDbContext _db;
    public SimulationRepository(UniversityDbContext db) => _db = db;

    public async Task<Guid> CreateSessionAsync(
        IStrategy strategy, Lesson[] lessons, int semesterDays, CancellationToken ct = default)
    {
        var sessionId = Guid.NewGuid();
        var session = new SessionEntity
        {
            Id = sessionId,
            StartedAt = DateTime.UtcNow,
            SemesterDays = semesterDays,
            CurrentDay = 0,
            IsFinished = false,
            IsDroppedOut = false,
            StrategyName = strategy.Name,
        };

        foreach (var lesson in lessons)
        {
            var (type, json) = RuleSerialization.Serialize(lesson.rule, lesson);
            session.Lessons.Add(new LessonConfigEntity
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Subject = lesson.Subject,
                RuleType = type,
                RuleParamsJson = json,
            });
        }

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync(ct);
        return sessionId;
    }

    public async Task SaveDayAsync(Guid sessionId, University uni, CancellationToken ct = default)
    {
        int day = uni.DroppedOut ? uni.Day : uni.Day - 1;

        if (day < 0) throw new InvalidOperationException("No day processed yet");

        var snapshot = new DaySnapshotEntity
        {
            SessionId = sessionId,
            Day = day,
            Satisfaction = uni.Student.Satisfaction,
            DroppedOut = uni.DroppedOut,
            Finished = uni.Finished,
            LastSubject = uni.Student.LastSubject,
        };

        for (int s = 0; s < Enum.GetValues<Subject>().Length; s++)
        {
            snapshot.Subjects.Add(new SubjectDayEntity
            {
                Subject = (Subject)s,
                WasPresent = uni.Student.DaysPresent[day, s],
                WasAsked = uni.Student.DaysWasAsked[day, s],
            });
        }

        _db.DaySnapshots.Add(snapshot);

        var session = await _db.Sessions.FindAsync([sessionId], ct)
            ?? throw new InvalidOperationException("Session not found");
        session.CurrentDay = uni.Day;
        session.IsFinished = uni.Finished;
        session.IsDroppedOut = uni.DroppedOut;

        await _db.SaveChangesAsync(ct);
    }

    public async Task<University> LoadAsync(
        Guid sessionId, Func<string, IStrategy> strategyFactory, CancellationToken ct = default)
    {
        var session = await _db.Sessions
            .Include(x => x.Lessons)
            .Include(x => x.Days).ThenInclude(d => d.Subjects)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == sessionId, ct)
            ?? throw new InvalidOperationException($"Session {sessionId} not found");

        var strategy = strategyFactory(session.StrategyName);
        var student = new Student(strategy, session.SemesterDays);

        foreach (var day in session.Days.OrderBy(d => d.Day))
        {
            foreach (var sd in day.Subjects)
            {
                student.DaysPresent[day.Day, (int)sd.Subject] = sd.WasPresent;
                student.DaysWasAsked[day.Day, (int)sd.Subject] = sd.WasAsked;
            }
            student.Satisfaction = day.Satisfaction;
            student.LastSubject = day.LastSubject ?? student.LastSubject;
        }

        var lessons = session.Lessons
            .OrderBy(x => (int)x.Subject)
            .Select(cfg =>
            {
                var (rule, a, b) = RuleSerialization.Deserialize(cfg.RuleType, cfg.RuleParamsJson, cfg.Subject);
                return new Lesson(cfg.Subject, rule, a, b);
            })
            .ToArray();

        return University.RestoreFrom(
            student, lessons,
            session.CurrentDay, session.SemesterDays,
            session.IsDroppedOut, session.IsFinished);
    }

    public Task<Guid?> FindUnfinishedAsync(CancellationToken ct = default) =>
        _db.Sessions
           .Where(s => !s.IsFinished && !s.IsDroppedOut)
           .OrderByDescending(s => s.StartedAt)
           .Select(s => (Guid?)s.Id)
           .FirstOrDefaultAsync(ct);
}