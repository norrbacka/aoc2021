using System.Reflection;

public static class Day12
{
    private static async Task<List<string>> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToListAsync();

    record Path(string From, string To);

    static Path[] ToPaths(this List<string> input) =>
        input.SelectMany(line =>
        {
            var split = line.Split("-");
            return new[] { new Path(split[0], split[1]), new Path(split[1], split[0]) };
        }).ToArray();

    static readonly Func<string[], string, bool> IsStart = (string[] _, string to) => 
        to == "start";

    static readonly Func<string[], string, bool> IsSmallCaveAndAlreadyVisited = (string[] visited, string to) =>
    {
        var isSmallCave = to == to.ToLower();
        var alreadyVisited =
            visited.Count(n => n == to) == 1;
        return isSmallCave && alreadyVisited;
    };

    static readonly Func<string[], string, bool> IsSmallCaveAndAlreadyVisitedTwice = (string[] visited, string to) =>
    {
        var isSmallCave = to == to.ToLower();
        var alreadyVisitedTwice = visited.Count(n => n == to) == 2;
        return isSmallCave && alreadyVisitedTwice;
    };

    static readonly Func<string[], string, bool> IsSmallCaveAndAlreadyVisitedOtherCaveTwice = (string[] visited, string to) =>
    {
        var isSmallCave = to == to.ToLower();
        var hasVisitedAnySmallCaveTwice = visited.Any(x =>
                x == x.ToLower() &&
                visited.Count(y => x == y) == 2);
        var alreadyVisitedOnce = visited.Count(n => n == to) == 1;
        return isSmallCave && alreadyVisitedOnce && hasVisitedAnySmallCaveTwice;
    };

    static (string[] visited, IList<string[]> completed) Traverse(this ILookup<string, Path> paths, params Func<string[], string, bool>[] Filters) =>
        paths.Traverse(new string[] { "start" }, new List<string[]>(), "start", Filters);

    static (string[] visited, IList<string[]> completed) Traverse(this ILookup<string, Path> paths, string[] visited, IList<string[]> completed,  string current, params Func<string[], string, bool>[] Filters)
    {
        if (current == "end") completed.Add(visited.ToArray());
        else
        {
            foreach (var to in paths[current].Select(p => p.To))
            {
                var newPath = visited.ToArray();
                if (Filters.Any(f => f(newPath, to))) continue;
                newPath = newPath.Append(to).ToArray();
                (newPath, completed) = Traverse(paths, newPath, completed, to, Filters);
            }
        }        
        return (visited, completed);
    }

    public static async Task<object> One() =>
        (await GetInput())
        .ToPaths()
        .ToLookup(p => p.From)
        .Traverse(IsSmallCaveAndAlreadyVisited)
        .completed.Count();

    public static async Task<object> Two() =>
        (await GetInput())
        .ToPaths()
        .ToLookup(p => p.From)
        .Traverse(
            IsStart,
            IsSmallCaveAndAlreadyVisitedTwice,
            IsSmallCaveAndAlreadyVisitedOtherCaveTwice)
        .completed.Count();
}