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
    public void NextInt_ReturnsQueuedValuesInOrder()
    {
        var src = new FakeRandomSource().WithInts(1, 3, 5);
        Assert.That(src.Next(0, 10), Is.EqualTo(1));
        Assert.That(src.Next(0, 10), Is.EqualTo(3));
        Assert.That(src.Next(0, 10), Is.EqualTo(5));
    }

    [Test]
    public void NextDouble_EmptyQueue_Throws()
    {
        var src = new FakeRandomSource();
        Assert.Throws<InvalidOperationException>(() => src.NextDouble());
    }

    [Test]
    public void SameConfiguredValues_ProduceSameSequence()
    {
        var a = new FakeRandomSource().WithDoubles(0.2, 0.4).WithInts(7);
        var b = new FakeRandomSource().WithDoubles(0.2, 0.4).WithInts(7);

        Assert.That(a.NextDouble(), Is.EqualTo(b.NextDouble()));
        Assert.That(a.Next(0, 100), Is.EqualTo(b.Next(0, 100)));
        Assert.That(a.NextDouble(), Is.EqualTo(b.NextDouble()));
    }
}