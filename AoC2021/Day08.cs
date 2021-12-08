using System.Reflection;
using WinstonPuckett.PipeExtensions;


public static class Day08
{
    static IEnumerable<string[][]> GroupedByTwo(IEnumerable<string> data)
    {
        var group = data
            .Select((x, i) => (value: x.Replace("|", "").Trim(), index: i))
            .ToLookup(x => x.index / 2, x => x.value);
        foreach(var key in group.Select(g => g.Key))
        {
            yield return group[key].Select(g => g.Split(" ").ToArray()).ToArray();
        }
    }
        

    private static async Task<string[][][]> GetInput() =>
        (await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Split(" | ").Select(t => t.Trim().Split(" ").Select(c => c.Trim()).ToArray()).ToArray())
        .ToListAsync())
        .ToArray();

    public static async Task<object> One() {
        var input = await GetInput();
        var onlyOutputs = input.SelectMany(i => i[1]).ToArray();
        var ones = onlyOutputs.Count(o => o.Length == 2);
        var fours = onlyOutputs.Count(o => o.Length == 4);
        var sevens = onlyOutputs.Count(o => o.Length == 3);
        var eights = onlyOutputs.Count(o => o.Length == 7);
        var sum = ones + fours + sevens + eights;
        return sum;
    }

    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
