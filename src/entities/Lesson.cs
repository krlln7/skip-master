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
    public Lesson(Subject subject, IRule rule, Subject subjectA, Subject subjectB)
    {
        Subject = subject;
        this.rule = rule;
        SubjectA = subjectA;
        SubjectB = subjectB;
    }

    [SetsRequiredMembers]
    public Lesson(Subject subj) : this(subj, new TrueRandomSource(), explicitRule: null) { }

    [SetsRequiredMembers]
    public Lesson(Subject subj, IRandomSource random, IRule? explicitRule = null)
    {
        Subject = subj;
        rule = explicitRule
            ?? (IRule)Activator.CreateInstance(TypesResolver.ResolveRandomly<IRule>(random))!;
        this.Subject = Subject;
        if (rule is ABRule || rule is ARule)
        {
            int n = Enum.GetValues<Subject>().Length;
            SubjectA = (Subject)random.Next(0, n);
            SubjectB = (Subject)(((int)SubjectA + 1 + random.Next(0, n - 1)) % n);
        }
        else
        {
            SubjectA = subj;
            SubjectB = subj;
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