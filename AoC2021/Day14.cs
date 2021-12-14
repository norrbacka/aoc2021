using System.Numerics;
using System.Reflection;
using WinstonPuckett.PipeExtensions;
using static MoreLinq.Extensions.PairwiseExtension;

public static class Day14
{
    private static async Task<string[]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToArrayAsync();

    record Rule(char First, char Second, char Insertion);

    static char[] GetTemplate(this string[] inputs) =>
        inputs
        .First()
        .Select(c => c)
        .ToArray();

    static IDictionary<(char a, char b), Rule> GetRules(this string[] inputs) =>
        inputs
        .Skip(2)
        .Select(i =>
        {
            var split = i.Split(" -> ");
            var rule = new Rule(split[0][0], split[0][1], split[1][0]);
            return rule;
        })
        .ToDictionary(x => (x.First, x.Second));

    static IDictionary<char, BigInteger> GetPolymerCounter(this char[] template) =>
        template
        .ToLookup(t => t)
        .ToDictionary(t => t.Key, t => (BigInteger)t.Count());

    static IDictionary<(char a, char b), BigInteger> GetPairCounter(this char[] template) =>
        template
        .Pairwise((a, b) => (a, b))
        .ToLookup(t => t)
        .ToDictionary(t => t.Key, t => (BigInteger)t.Count());

    static void Simulate(ref IDictionary<(char a, char b), BigInteger> count, ref IDictionary<char, BigInteger> polymerCount, IDictionary<(char a, char b), Rule> rules)
    {
        foreach (var (pair, pairCount) in new Dictionary<(char a, char b), BigInteger>(count))
        {
            if (!rules.ContainsKey(pair)) continue;

            count[pair] -= pairCount;
            var polymer = rules[pair].Insertion;

            if (!count.ContainsKey((pair.a, polymer))) count.Add((pair.a, polymer), 0);
            count[(pair.a, polymer)] += pairCount;

            if (!count.ContainsKey((polymer, pair.b))) count.Add((polymer, pair.b), 0);
            count[(polymer, pair.b)] += pairCount;

            if (!polymerCount.ContainsKey(polymer)) polymerCount.Add(polymer, 0);
            polymerCount[polymer] += pairCount;
        }
    }
    private static async Task<object> Simulate(int rounds) =>
        (await GetInput())
        .Pipe(input => (
            input.GetTemplate(), 
            input.GetRules()))
        .Pipe((template, rules) => (
            template.GetPolymerCounter(),
            template.GetPairCounter(),
            rules
        ))
        .Pipe((polymerCount, pairCount, rules) =>
        {
            for (int i = 1; i <= rounds; i++) Simulate(ref pairCount, ref polymerCount, rules);
            var mostCommon = polymerCount.Max(x => x.Value);
            var leastCommon = polymerCount.Min(x => x.Value);
            return mostCommon - leastCommon;
        });

    public static async Task<object> One() => await Simulate(10);
    public static async Task<object> Two() => await Simulate(40);
}