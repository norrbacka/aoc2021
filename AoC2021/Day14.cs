using LanguageExt;
using System.Numerics;
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

    static char[] GetTemplate(this string[] inputs) =>
        inputs.First().Select(c => c).ToArray();

    static Rule[] GetRules(this string[] inputs) =>
        inputs.Skip(2).Select(i =>
        {
            var split = i.Split(" -> ");
            var rule = new Rule(split[0][0], split[0][1], split[1][0]);
            return rule;
        }).ToArray();

    static void Simulate(ref Dictionary<(char a, char b), BigInteger> count, ref Dictionary<char, BigInteger> polymerCount, Dictionary<(char a, char b), Rule> rules)
    {
        foreach (var (pair, pairCount) in new Dictionary<(char a, char b), BigInteger>(count))
        {
            var maybeRule = rules.ContainsKey(pair) ? rules[pair] : Option<Rule>.None;
            if (maybeRule.IsSome)
            {
                count[pair] -= pairCount;

                var rule = maybeRule.Some(r => r).None(() => throw new InvalidOperationException());
                var polymer = rule.Insertion;
                if (!count.ContainsKey((pair.a, polymer)))
                {
                    count.Add((pair.a, polymer), 0);
                }
                count[(pair.a, polymer)] += pairCount;

                if (!count.ContainsKey((polymer, pair.b)))
                {
                    count.Add((polymer, pair.b), 0);
                }
                count[(polymer, pair.b)] += pairCount;

                if (!polymerCount.ContainsKey(polymer))
                {
                    polymerCount.Add(polymer, 0);
                }
                polymerCount[polymer] += pairCount;
            }
        }
    }
    private static async Task<object> Simulate(int rounds)
    {
        var input = await GetInput();
        var template = input.GetTemplate();
        var rules = input.GetRules().ToDictionary(x => (x.First, x.Second));
        var polymerCount = template.ToLookup(r => r).Select(l => (l.Key, Count: l.Count())).ToDictionary(x => x.Key, x => (BigInteger)x.Count);
        var countedDict = template.Pairwise((a, b) => (a, b)).ToLookup(r => r).Select(l => (l.Key, Count: l.Count())).ToDictionary(x => x.Key, x => (BigInteger)x.Count);
        for (int i = 1; i <= rounds; i++)
        {
            Simulate(ref countedDict, ref polymerCount, rules);
        }
        var mostCommon = polymerCount.Max(x => x.Value);
        var leastCommon = polymerCount.Min(x => x.Value);
        var diff = mostCommon - leastCommon;
        return diff;
    }

    public static async Task<object> One() => await Simulate(10);
    public static async Task<object> Two() => await Simulate(40);
}
