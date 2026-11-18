namespace proj_tests;

[TestFixture]
public class FakeRandomSourceTests
{
    [Test]
    public void NextDouble_ReturnsQueuedValuesInOrder()
    {
        var src = new FakeRandomSource().WithDoubles(0.1, 0.5, 0.9);
        Assert.That(src.NextDouble(), Is.EqualTo(0.1));
        Assert.That(src.NextDouble(), Is.EqualTo(0.5));
        Assert.That(src.NextDouble(), Is.EqualTo(0.9));
    }

    [Test]
    public void NextDouble_EmptyQueue_Throws()
    {
        var src = new FakeRandomSource();
        Assert.Throws<InvalidOperationException>(() => src.NextDouble());
    }
}