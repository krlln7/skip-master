using Microsoft.EntityFrameworkCore;

using Testcontainers.PostgreSql;

namespace proj_tests;

sealed class PostgresFixture
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder("postgres:17")
            .WithDatabase("course_proj_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var db = CreateContext();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    public UniversityDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;
        return new UniversityDbContext(opts);
    }
}