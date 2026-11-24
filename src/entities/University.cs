class University
{
    public int SemesterDays { get; set; }
    public Lesson[] Classes { get; set; }

    public int Day { get; private set; }
    public bool DroppedOut { get; private set; }
    public bool Finished { get; private set; }

    public Student Student { get; private set; }

    public University(IStrategy strategy, int semDays = 100, Lesson[]? classes = null)
    {
        if (classes == null)
        {
            Classes = Enum.GetValues<Subject>()
                      .Select(subject => new Lesson(subject, new TrueRandomSource()))
                      .ToArray();
        }
        else
        {
            Classes = classes;
        }

        SemesterDays = semDays;

        Student = new Student(strategy, semDays);
    }

    internal University(Student student, Lesson[] classes, int semesterDays)
    {
        Student = student;
        Classes = classes;
        SemesterDays = semesterDays;
    }

    public static University RestoreFrom(
        Student student, Lesson[] classes,
        int currentDay, int semesterDays,
        bool droppedOut, bool finished)
    {
        var uni = new University(student, classes, semesterDays)
        {
            Day = currentDay,
            DroppedOut = droppedOut,
            Finished = finished
        };
        return uni;
    }

    // dropped out or not today?
    public bool RunOneDay()
    {
        if (DroppedOut || Finished) return false;

        Student.DoDay(Day); // decides to skip or not to skip

        for (int l = 0; l < Enum.GetValues<Subject>().Length; l++)
        {
            Classes[l].DoClass(Day, Student);

            //drop out
            if (Student.Satisfaction == 0)
            {
                DroppedOut = true;
                return false;
            }
        }
        Day++;
        if (Day >= SemesterDays) Finished = true;

        return !Finished;
    }
}