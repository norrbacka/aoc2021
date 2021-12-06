using WinstonPuckett.PipeExtensions;

public static class Day6
{
    private static async Task<int[]> GetInput() =>
        (await Inputs
        .Read("inputs/day6.txt")
        .Select(text => text.Split(",").Select(s => int.Parse(s)))
        .ToListAsync())
        .SelectMany(x => x)
        .ToArray();

    public static IEnumerable<int> ProceedOneDay(IEnumerable<int> input)
    {
        var childrenCount = 0;
        foreach (var i in input)
        {
            if (i > 0)
            {
                var di = i - 1;
                yield return di;
            }
            else
            {
                childrenCount++;
                var itself = 6;
                yield return itself;
            }
        }
        foreach(var _ in Enumerable.Range(0, childrenCount))
        {
            yield return 8;
        }
    }

    public static int[][] ProceedDays(IEnumerable<int> input, int day, int stop, int[][] result)
    {
        if (day <= stop)
        {
            var newInput = ProceedOneDay(input).ToArray();
            var newResult = result.Select(r => r).Append(newInput).ToArray();
            return ProceedDays(newInput, day + 1, stop, newResult);
        }
        return result;
    }

    public static async Task<object> One() =>
        (await GetInput())
        .Pipe(input => ProceedDays(input, 1, 80, new int[][] { }))
        .Last()
        .Count();

    public static async Task<object> Two() => "bar";
}
