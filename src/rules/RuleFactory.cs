sealed class RuleFactory
{
    private RuleFactory() { }
    public static RuleFactory Instance { get { return Nested.instance; } }

    private class Nested
    {
        static Nested() { }
        internal static readonly RuleFactory instance = new RuleFactory();
    }
}