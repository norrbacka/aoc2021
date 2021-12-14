using LanguageExt;
using System.Reflection;
using static MoreLinq.Extensions.PairwiseExtension;

public static class Day14
{
    private static async Task<string[]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToArrayAsync();

    record Rule(char First, char Second, char Insertion)
    {
        public bool Covers(char a, char b) => 
            First == a && Second == b;
    };

    static Option<Rule> Get(this Rule[] rules, char a, char b) =>
        rules.Any(r => r.Covers(a,b)) ? 
            rules.Single(r => r.Covers(a, b)) :
            Option<Rule>.None;

    static char[] GetTemplate(this string[] inputs) =>
        inputs.First().Select(c => c).ToArray();

    static Rule[] GetRules(this string[] inputs) =>
        inputs.Skip(2).Select(i =>
        {
            var split = i.Split(" -> ");
            var rule = new Rule(split[0][0], split[0][1], split[1][0]);
            return rule;
        }).ToArray();   

    static IEnumerable<char> SimultaneouslyInsert(char[] template, Rule[] rules)
    {
        foreach(var (a,b) in template.Pairwise((a,b) => (a,b)))
        {
            yield return a;
            var maybeRule = rules.Get(a,b);
            if(maybeRule.IsSome)
            {
                var rule = maybeRule.Some(r => r).None(() => throw new InvalidOperationException());
                yield return rule.Insertion;
            }
        }
        yield return template.Last();
    }

    public static async Task<object> One()
    {
        var input = await GetInput();
        var template = input.GetTemplate();
        var rules = input.GetRules();
        for (int i = 1; i <= 10; i++)
        {
            template = SimultaneouslyInsert(template, rules).ToArray();
        }
        var countedDict = template.ToLookup(r => r).Select(l => (l.Key, Count: l.Count())).ToDictionary(x => x.Key, x => x.Count);
        var mostCommon = countedDict.Max(x => x.Value);
        var leastCommon = countedDict.Min(x => x.Value);
        var diff = mostCommon - leastCommon;
        return diff;
    }
    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
