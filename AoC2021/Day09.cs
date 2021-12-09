using System.Reflection;
using WinstonPuckett.PipeExtensions;

public static class Day09
{
    private static async Task<int[][]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Select(i => int.Parse(i.ToString())).ToArray())
        .ToArrayAsync();



    public static async Task<object> One()
    {
        var input = await GetInput();
        var inputAndAdjacents = new List<((int x, int y) position, int value, int[] adjacents, bool isLowPoint)>();
        
        for(var i = 0; i< input.Length; i++) 
        for(var j = 0 ; j < input[i].Length; j++)
        {
            var position = (x: i, y: j);
            var value = input[i][j];

            var hasRowAbove = i > 0;
            var hasRowBelow = i < (input.Length - 1);
            var hasColumnLeft = j > 0;
            var hasColumnRight = j < (input[i].Length - 1);

            var adjacents = new int[] { }
            .Pipe(adj => hasRowAbove ? adj.Concat(new[]{ input[i-1][j] }).ToArray() : adj)
            .Pipe(adj => hasRowBelow ? adj.Concat(new[]{ input[i+1][j] }).ToArray() : adj)
            .Pipe(adj => hasColumnLeft ? adj.Concat(new[]{ input[i][j-1] }).ToArray() : adj)
            .Pipe(adj => hasColumnRight ? adj.Concat(new[]{ input[i][j+1] }).ToArray() : adj)
            .ToArray();

            var isLowPoint = adjacents.All(adj => adj > value);
            inputAndAdjacents.Add((position, value, adjacents, isLowPoint));
        }

        var lowPoints = inputAndAdjacents.Where(x => x.isLowPoint).ToArray();
        var sum = lowPoints.Sum(x => x.value + 1);
        return sum;
    }
    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
