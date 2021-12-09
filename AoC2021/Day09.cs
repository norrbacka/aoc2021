using System.Diagnostics;
using System.Reflection;
using WinstonPuckett.PipeExtensions;

public static class Day09
{
    private static async Task<long[][]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Select(i => long.Parse(i.ToString())).ToArray())
        .ToArrayAsync();

    private static IEnumerable<((long x, long y) position, long value, long[] adjacents, bool isLowPolong)> GetData(long[][] input)
    {
        for (var i = 0; i < input.Length; i++)
        for (var j = 0; j < input[i].Length; j++)
            {
                var position = (x: i, y: j);
                var value = input[i][j];

                var hasRowAbove = HasTop(i);
                var hasRowBelow = HasBottom(input, i);
                var hasColumnLeft = HasLeft(j);
                var hasColumnRight = HasRight(input, i, j);

                var adjacents = new long[] { }
                .Pipe(adj => hasRowAbove ? adj.Concat(new[] { input[i - 1][j] }).ToArray() : adj)
                .Pipe(adj => hasRowBelow ? adj.Concat(new[] { input[i + 1][j] }).ToArray() : adj)
                .Pipe(adj => hasColumnLeft ? adj.Concat(new[] { input[i][j - 1] }).ToArray() : adj)
                .Pipe(adj => hasColumnRight ? adj.Concat(new[] { input[i][j + 1] }).ToArray() : adj)
                .ToArray();

                var isLowPolong = adjacents.All(adj => adj > value);
                yield return (position, value, adjacents, isLowPolong);
            }
    }


    private static bool HasTop(long i) => i > 0;
    private static bool HasRight(long[][] input, long i, long j) => j < (input[i].Length - 1);
    private static bool HasBottom(long[][] input, long i) => i < (input.Length - 1);
    private static bool HasLeft(long j) => j > 0;


    public static async Task<object> One()
    {
        var input = await GetInput();
        var inputAndAdjacents = GetData(input);
        var lowPolongs = inputAndAdjacents.Where(x => x.isLowPolong).ToArray();
        var sum = lowPolongs.Sum(x => x.value + 1);
        return sum;
    }

    static IEnumerable<(long i, long j)> BasinSize(long[][] input, long i, long j, IEnumerable<(long i, long j)> sizes)
    {
        if(sizes.Contains((i,j)) && sizes.Count() > 1)
        {
            return sizes;
        }
        sizes = sizes.ToList().Concat(new[] { (i, j) });
        var value = input[i][j];

        var t = HasTop(i);
        var r = HasRight(input, i, j);
        var b = HasBottom(input, i);
        var l = HasLeft(j);

        if (t && (input[i - 1][j] > value && input[i - 1][j] != 9))
        {
            sizes = BasinSize(input, i - 1, j, sizes);
        }
        if (b && (input[i + 1][j] > value) && (input[i + 1][j] != 9))
        {
            sizes = BasinSize(input, i + 1, j, sizes);
        }
        if (l && (input[i][j - 1] > value) && input[i][j - 1] != 9)
        {
            sizes = BasinSize(input, i, j - 1, sizes);
        }
        if (r && (input[i][j + 1] > value) && input[i][j + 1] != 9)
        {
            sizes = BasinSize(input, i, j + 1, sizes);
        }

        return sizes;
    }

    public static async Task<object> Two()
    {
        var input = await GetInput();
        var inputAndAdjacents = GetData(input);
        var lowPolongs = inputAndAdjacents.Where(x => x.isLowPolong).ToArray();
        var basins = lowPolongs.Select(l => 
            BasinSize(input, l.position.x, l.position.y, new (long, long)[] { (l.position.x, l.position.y) })
            .Distinct()
        ).ToArray();
        var top3largetsBasins = 
            basins
            .OrderByDescending(y => y.Count())
            .Take(3)
            .ToArray();
        var basinsSum =
            top3largetsBasins
            .Aggregate(1, (x, y) => x * y.Count());
        return basinsSum;
    }
}
