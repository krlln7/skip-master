using Microsoft.Extensions.Logging;

interface IProgressReporter
{
    void Report(int day, Student student);
}

class LoggerProgressReporter : IProgressReporter
{
    private readonly ILogger<LoggerProgressReporter> _logger;
    public LoggerProgressReporter(ILogger<LoggerProgressReporter> logger) => _logger = logger;

    public void Report(int day, Student student)
    {
        float avg = day > 0 ? student.Satisfaction / day : 0f;
        _logger.LogInformation(
            "Day {Day}: satisfaction = {Satisfaction}, avg/day = {Average:F2}",
            day, student.Satisfaction, avg);

        if (student.Satisfaction == 0)
        {
            _logger.LogInformation("Semester finished after {Days} days.", day);
            _logger.LogInformation("Student was killed by " + Enum.GetName(typeof(Subject), student.LastSubject));
        }
    }
}