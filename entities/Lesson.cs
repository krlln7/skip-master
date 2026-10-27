using System.Diagnostics.CodeAnalysis;

class Lesson
{
    public Subject SubjectA { get; }
    public Subject SubjectB { get; }

    private static readonly Type[] RuleTypes =
    typeof(IRule).Assembly
        .GetTypes()
        .Where(t => typeof(IRule).IsAssignableFrom(t) && !t.IsInterface)
        .ToArray();
    public required IRule rule;

    public required Subject Subject { get; init; }

    [SetsRequiredMembers]
    public Lesson(Subject subj)
    {
        var ruleType = TypesResolver.ResolveRandomly<IRule>();
        rule = (IRule)Activator.CreateInstance(ruleType)!;

        Subject = subj;

        if (rule is ABRule || rule is ARule)
        {
            int subjNum = Enum.GetValues<Subject>().Length;
            SubjectA = (Subject)Random.Shared.Next(subjNum);
            SubjectB = (Subject)(((int)SubjectA + 1 + Random.Shared.Next(subjNum - 1)) % subjNum); // constraints: A != B
        }
    }

    public void DoClass(int day, Student student)
    {
        bool isQuestioned = rule.Decide(day, student, this);
        if (!isQuestioned)
        {
            student.DaysWasAsked[day, (int)Subject] = false;
            return;
        }

        if (!student.DaysPresent[day, (int)Subject])
        {
            // drop out
            student.Satisfaction = 0;
            student.LastSubject = Subject;
        }
        student.DaysWasAsked[day, (int)Subject] = true;
    }
}