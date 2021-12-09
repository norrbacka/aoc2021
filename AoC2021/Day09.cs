using System.Reflection;
using WinstonPuckett.PipeExtensions;

public static class Day09
{
    static IEnumerable<long> Concat(this IEnumerable<long> list, long value) => list.Concat(new[] { value });

    private static async Task<long[][]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Select(i => long.Parse(i.ToString())).ToArray())
        .ToArrayAsync();

    record Location((long X, long Y) Position, long Value, long[] Adjacents, bool IsLowerPointer);

    private static IEnumerable<Location> GetLocations(long[][] input) =>
        Enumerable.Range(0, input.Length).SelectMany(i =>
        Enumerable.Range(0, input[i].Length).Select(j =>
        {
            var position = (x: (long)i, y: (long)j);
            var value = input[i][j];
            var adjacents = 
               (HasTop(i) ? new long[]{GetTop(input, i, j)} : Array.Empty<long>()).Pipe(adj =>
                HasBottom(input, i) ? adj.Concat(GetBelow(input, i, j)).ToArray() : adj).Pipe(adj =>
                HasLeft(j) ? adj.Concat(GetLeft(input, i, j)).ToArray() : adj).Pipe(adj =>
                HasRight(input, i, j) ? adj.Concat(GetRight(input, i, j)).ToArray() : adj)
            .ToArray();
            var isLowerPointer = adjacents.All(adj => adj > value);
            return new Location(position, value, adjacents, isLowerPointer);
        }));

    private static bool HasTop(long i) => i > 0;
    private static bool HasRight(long[][] input, long i, long j) => j < (input[i].Length - 1);
    private static bool HasBottom(long[][] input, long i) => i < (input.Length - 1);
    private static bool HasLeft(long j) => j > 0;
    private static long GetTop(long[][] input, long i, long j) => input[i - 1][j];
    private static long GetRight(long[][] input, long i, long j) => input[i][j + 1];
    private static long GetBelow(long[][] input, long i, long j) => input[i + 1][j];
    private static long GetLeft(long[][] input, long i, long j) => input[i][j - 1];
    private static bool TopIsHigherLocation(long[][] input, long i, long j, long value) => GetTop(input, i, j) > value;
    private static bool RightIsHigherLocation(long[][] input, long i, long j, long value) => GetRight(input, i, j) > value;
    private static bool BelowIsHigherLocation(long[][] input, long i, long j, long value) => GetBelow(input, i, j) > value;
    private static bool LeftIsHigherLocation(long[][] input, long i, long j, long value) => GetLeft(input, i, j) > value;

    public static async Task<object> One() =>
        (await GetInput())
        .Pipe(GetLocations)
        .Pipe(inputAndAdjacents => inputAndAdjacents.Where(x => x.IsLowerPointer))
        .Sum(x => (x.Value + 1));

    static IEnumerable<(long i, long j)> GetBasin(long[][] input, long i, long j, IEnumerable<(long i, long j)> sizes)
    {
        if (sizes.Contains((i, j)) && sizes.Count() > 1) return sizes;

        sizes = sizes.ToList().Concat(new[] { (i, j) });
        var value = input[i][j];

        var GoUp = HasTop(i) && TopIsHigherLocation(input, i, j, value) && GetTop(input, i, j) != 9;
        if (GoUp) sizes = GetBasin(input, i - 1, j, sizes);

        var GoDown = HasBottom(input, i) && BelowIsHigherLocation(input, i, j, value) && GetBelow(input, i, j) != 9;
        if (GoDown) sizes = GetBasin(input, i + 1, j, sizes);

        var GoLeft = HasLeft(j) && LeftIsHigherLocation(input, i, j, value) && GetLeft(input, i, j) != 9;
        if (GoLeft) sizes = GetBasin(input, i, j - 1, sizes);

        var GoRight = HasRight(input, i, j) && RightIsHigherLocation(input, i, j, value) && GetRight(input, i, j) != 9;
        if (GoRight) sizes = GetBasin(input, i, j + 1, sizes);

        return sizes;
    }

    public static async Task<object> Two() =>
        (await GetInput())
        .Pipe(input => 
            GetLocations(input)
            .Where(x => x.IsLowerPointer)
            .Select(l =>
                GetBasin(input, l.Position.X, l.Position.Y, new (long, long)[] { (l.Position.X, l.Position.Y) })
                .Distinct()))
        .OrderByDescending(y => y.Count())
        .Take(3)
        .Aggregate(1, (x, y) => x * y.Count());
}