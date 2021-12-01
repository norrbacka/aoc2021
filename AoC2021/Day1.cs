using LanguageExt;
using static MoreLinq.Extensions.PairwiseExtension;
using WinstonPuckett.PipeExtensions;

public static class Day1
{
    private static async Task<IList<int>> GetInput() => 
        await Inputs
        .Read("inputs/day1.txt")
        .Select(i => int.Parse(i))
        .ToListAsync();

    public static async Task<string> A() =>
        (await GetInput())
        .Pipe(CountIncreases)
        .Pipe(x => x.ToString());

    public static async Task<string> B() =>
        (await GetInput())
        .Pipe(ToGroupsOfThree)
        .Pipe(ToSums)
        .Pipe(CountIncreases)
        .Pipe(x => x.ToString());

    public static int CountIncreases(IList<int> inputs) => 
        inputs
        .Prepend(int.MaxValue)
        .Pairwise((previous, current) => current > previous ? 1 : 0)
        .Sum();

    public static IEnumerable<IList<int>> ToGroupsOfThree(IList<int> inputs)
    {
        List<int> a = new(), b = new(), c = new();
        int laps = 1;
        foreach (int i in inputs)
        {
            if (laps >= 1) a.Add(i);
            if (laps >= 2) b.Add(i); 
            if (laps++ >= 3) c.Add(i);
            if (a.Count == 3) { yield return a; a.Clear(); }
            if (b.Count == 3) { yield return b; b.Clear(); }
            if (c.Count == 3) { yield return c; c.Clear(); }
        }
    }

    public static IList<int> ToSums(IEnumerable<IList<int>> inputsGrouped) =>
        inputsGrouped
        .Select(i => i.Sum())
        .ToList();
}