namespace proj_tests;

[TestFixture]
public class FifthyRuleTests
{
    private static readonly (int day, bool expected)[] Cases =
    {
        (0,   true),
        (1,   false),
        (25,  false),
        (49,  false),
        (50,  true),
        (51,  false),
        (99,  false),
        (100, true),
        (150, true),
    };
}