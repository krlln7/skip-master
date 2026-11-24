using System.Text.Json;

static class RuleSerialization
{
    private record RuleParams(int? A = null, int? B = null, bool? Result = null, int? Seed = null);

    public static (string Type, string Json) Serialize(IRule rule, Lesson lesson)
    {
        return rule switch
        {
            FixedRule f => ("FixedRule", JsonSerializer.Serialize(new RuleParams(Result: f.Result))),
            ABRule => ("ABRule", JsonSerializer.Serialize(new RuleParams(A: (int)lesson.SubjectA, B: (int)lesson.SubjectB))),
            ARule => ("ARule", JsonSerializer.Serialize(new RuleParams(A: (int)lesson.SubjectA))),
            FifthyRule f => ("FifthyRule", JsonSerializer.Serialize(new RuleParams(Seed: f.Seed))),
            _ => throw new NotSupportedException($"Rule {rule.GetType().Name} is not serializable")
        };
    }

    public static (IRule Rule, Subject A, Subject B) Deserialize(string type, string json, Subject fallback)
    {
        var p = JsonSerializer.Deserialize<RuleParams>(json) ?? new();
        return type switch
        {
            "FixedRule" => (new FixedRule { Result = p.Result ?? false }, fallback, fallback),
            "ABRule" => (new ABRule(), (Subject)(p.A ?? 0), (Subject)(p.B ?? 1)),
            "ARule" => (new ARule(), (Subject)(p.A ?? 0), fallback),
            "FifthyRule" => (new FifthyRule { Seed = p.Seed ?? 0 }, fallback, fallback),
            _ => throw new NotSupportedException(type)
        };
    }
}