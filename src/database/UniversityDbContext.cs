using Microsoft.EntityFrameworkCore;

public class UniversityDbContext : DbContext
{
    public DbSet<SessionEntity> Sessions => Set<SessionEntity>();
    public DbSet<LessonConfigEntity> LessonConfigs => Set<LessonConfigEntity>();
    public DbSet<DaySnapshotEntity> DaySnapshots => Set<DaySnapshotEntity>();
    public DbSet<SubjectDayEntity> SubjectDays => Set<SubjectDayEntity>();

    public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<SessionEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.StrategyName).IsRequired().HasMaxLength(100);
            e.HasMany(x => x.Lessons).WithOne(x => x.Session!)
                .HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Days).WithOne(x => x.Session!)
                .HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<LessonConfigEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.RuleType).IsRequired().HasMaxLength(100);
            e.Property(x => x.RuleParamsJson).IsRequired().HasColumnType("jsonb");
            e.HasIndex(x => new { x.SessionId, x.Subject }).IsUnique();
        });

        mb.Entity<DaySnapshotEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.SessionId, x.Day }).IsUnique();
            e.HasMany(x => x.Subjects).WithOne(x => x.DaySnapshot!)
                .HasForeignKey(x => x.DaySnapshotId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<SubjectDayEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.DaySnapshotId, x.Subject }).IsUnique();
        });
    }
}