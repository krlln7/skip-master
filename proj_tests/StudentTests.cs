namespace proj_tests;

[TestFixture]
public class StudentTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void DoDay_Pirojok_Gives_One_Point()
    {
        Student student = new Student(FixedStrategy.AttendAll());
        float beforeSat = student.Satisfaction;

        student.DoDay(0);

        Assert.That(student.Satisfaction, Is.GreaterThanOrEqualTo(beforeSat + 1));
    }

    [Test]
    public void DoDay_Should_Record_Presence_For_All_Subjects()
    {
        Student student = new Student(FixedStrategy.AttendAll());
        student.DoDay(0);

        for (int l = 0; l < Enum.GetValues<Subject>().Length; l++)
        {
            Assert.That(student.Attended(0, (Subject)l), Is.True, $"Subject {(Subject)l} should be noted as Attended (true)");
        }
    }
}