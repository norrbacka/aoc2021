using System.Diagnostics;
using System.Reflection;
using WinstonPuckett.PipeExtensions;

public static class Day09
{
    private static async Task<int[][]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Select(i => int.Parse(i.ToString())).ToArray())
        .ToArrayAsync();

    private static IEnumerable<((int x, int y) position, int value, int[] adjacents, bool isLowPoint)> GetData(int[][] input)
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

                var adjacents = new int[] { }
                .Pipe(adj => hasRowAbove ? adj.Concat(new[] { input[i - 1][j] }).ToArray() : adj)
                .Pipe(adj => hasRowBelow ? adj.Concat(new[] { input[i + 1][j] }).ToArray() : adj)
                .Pipe(adj => hasColumnLeft ? adj.Concat(new[] { input[i][j - 1] }).ToArray() : adj)
                .Pipe(adj => hasColumnRight ? adj.Concat(new[] { input[i][j + 1] }).ToArray() : adj)
                .ToArray();

                var isLowPoint = adjacents.All(adj => adj > value);
                yield return (position, value, adjacents, isLowPoint);
            }
    }


    private static bool HasTop(int i) => i > 0;
    private static bool HasRight(int[][] input, int i, int j) => j < (input[i].Length - 1);
    private static bool HasBottom(int[][] input, int i) => i < (input.Length - 1);
    private static bool HasLeft(int j) => j > 0;


    public static async Task<object> One()
    {
        var input = await GetInput();
        var inputAndAdjacents = GetData(input);
        var lowPoints = inputAndAdjacents.Where(x => x.isLowPoint).ToArray();
        var sum = lowPoints.Sum(x => x.value + 1);
        return sum;
    }

    static IEnumerable<(int i, int j, int v)> BasinSize(int[][] input, int i, int j, IEnumerable<(int i, int j, int v)> sizes)
    {
        var value = input[i][j];
        var newValue = (value + 1);
       
        if(newValue == 9)
        {
            return sizes;
        }

        var t = HasTop(i);
        var r = HasRight(input, i, j);
        var b = HasBottom(input, i);
        var l = HasLeft(j);

        if (t && input[i - 1][j] == newValue)
        {
            sizes = sizes.ToList().Concat(new[] { (i - 1, j, newValue) });
            //Debug.WriteLine($"↑ ({i},{j}) -> ({i-1},{j}) [{string.Join(", ", sizes.Select(s => s.v))}]");
            sizes = BasinSize(input, i - 1, j, sizes);
        }
        if (b && input[i + 1][j] == newValue)
        {
            sizes = sizes.ToList().Concat(new[] { (i + 1, j, newValue) });
            //Debug.WriteLine($"↓ ({i},{j}) -> ({i + 1},{j}) [{string.Join(", ", sizes.Select(s => s.v))}]");
            sizes = BasinSize(input, i + 1, j, sizes);
        }
        if (l && input[i][j - 1] == newValue)
        {
            sizes = sizes.ToList().Concat(new[] { (i, j-1, newValue) });
            //Debug.WriteLine($"← ({i},{j}) -> ({i},{j-1}) [{string.Join(", ", sizes.Select(s => s.v))}]");
            sizes = BasinSize(input, i, j - 1, sizes);
        }
        if (r && input[i][j + 1] == newValue)
        {
            sizes = sizes.ToList().Concat(new[] { (i, j + 1, newValue) });
            //Debug.WriteLine($"→ ({i},{j}) -> ({i},{j + 1}) [{string.Join(", ", sizes.Select(s => s.v))}]");
            sizes = BasinSize(input, i, j + 1, sizes);
        }

        return sizes;
    }

    public static async Task<object> Two()
    {
        var input = await GetInput();
        var inputAndAdjacents = GetData(input);
        var lowPoints = inputAndAdjacents.Where(x => x.isLowPoint).ToArray();
        var basins = lowPoints.Select(l => 
            BasinSize(input, l.position.x, l.position.y, new (int, int, int)[] { (l.position.x, l.position.y, l.value) })
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
