public class SessionEntity
{
    public Guid Id { get; set; }
    public DateTime StartedAt { get; set; }
    public int SemesterDays { get; set; }
    public int CurrentDay { get; set; }
    public bool IsFinished { get; set; }
    public bool IsDroppedOut { get; set; }
    public string StrategyName { get; set; } = "";
    public List<LessonConfigEntity> Lessons { get; set; } = new();
    public List<DaySnapshotEntity> Days { get; set; } = new();
}

public class LessonConfigEntity
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public SessionEntity? Session { get; set; }
    public Subject Subject { get; set; }
    public string RuleType { get; set; } = "";
    public string RuleParamsJson { get; set; } = "{}";
}

public class DaySnapshotEntity
{
    public long Id { get; set; }
    public Guid SessionId { get; set; }
    public SessionEntity? Session { get; set; }
    public int Day { get; set; }
    public float Satisfaction { get; set; }
    public bool DroppedOut { get; set; }
    public bool Finished { get; set; }
    public Subject? LastSubject { get; set; }
    public List<SubjectDayEntity> Subjects { get; set; } = new();
}

public class SubjectDayEntity
{
    public long Id { get; set; }
    public long DaySnapshotId { get; set; }
    public DaySnapshotEntity? DaySnapshot { get; set; }
    public Subject Subject { get; set; }
    public bool WasPresent { get; set; }
    public bool? WasAsked { get; set; }
}