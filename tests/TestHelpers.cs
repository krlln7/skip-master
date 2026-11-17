namespace proj_tests;

internal static class TestHelpers
{
    public static int SubjectCount => Enum.GetValues<Subject>().Length;

    public static Lesson CreateLesson(Subject subject, IRule rule)
    {
        var lesson = new Lesson(subject);
        lesson.rule = rule;
        return lesson;
    }

    public static Lesson[] CreateLessons(IRule rule)
        => Enum.GetValues<Subject>().Select(s => CreateLesson(s, rule)).ToArray();

    public static Lesson[] CreateLessonsWithOverride(
        IRule defaultRule, Subject overridden, IRule overrideRule)
    {
        var lessons = CreateLessons(defaultRule);
        lessons[(int)overridden] = CreateLesson(overridden, overrideRule);
        return lessons;
    }
}