public static class Day5
{
    private static async Task<List<((int x, int y) start, (int x, int y) end)>> GetInput() =>
        await Inputs
        .Read("inputs/day5.txt")
        .Select(text =>
        {
            var pairs = text.Split(" -> ");
            var startPair = pairs[0].Split(",");
            var endPair = pairs[1].Split(",");
            var x1y1 = (int.Parse(startPair[0]), int.Parse(startPair[1]));
            var x2y2 = (int.Parse(endPair[0]), int.Parse(endPair[1]));
            return (x1y1, x2y2);
        })  
        .ToListAsync();

    public static IEnumerable<(int x, int y)> GetLines(((int x, int y) start, (int x, int y) end) lineSegment)
    {
        var dx = lineSegment.start.x;
        var dy = lineSegment.start.y;
        var pair = () => (dx, dy);
        yield return pair();
        do
        {
            if(dx < lineSegment.end.x) dx++;
            if(dx > lineSegment.end.x) dx--;
            if(dy < lineSegment.end.y) dy++;
            if(dy > lineSegment.end.y) dy--;
            yield return pair();
        }
        while(pair() != lineSegment.end);
    }

    private static int[][] GetMap() =>
        Enumerable
        .Range(0, 1000)
        .Select(n =>
            Enumerable
            .Range(0, 1000)
            .Select(m => 0)
            .ToArray())
        .ToArray();

    private static int[][] UpdateMap(int[][] map, (int x, int y) coordinate)
    {
        map[coordinate.x][coordinate.y]++;
        return map;
    }

    public static async Task<object> GetOverlapsOverTwo(bool onlyHorizontalAndVertical)
    {
        var lineSegments = await GetInput();

        var onlyHorizontalAndVerticals = lineSegments
            .Where(p =>
                p.start.x == p.end.x ||
                p.start.y == p.end.y)
            .ToArray();

        var linesData =
            onlyHorizontalAndVertical ?
                lineSegments
                .Where(p =>
                    p.start.x == p.end.x ||
                    p.start.y == p.end.y
                ) :
                lineSegments;

        var lines =
            linesData
            .Select(ls => GetLines(ls).ToArray())
            .ToArray();

        var map = GetMap();
        var linesCoordinates = lines.SelectMany(l => l);
        foreach (var coordinate in linesCoordinates)
        {
            map = UpdateMap(map, coordinate);
        }

        var overlaps = map.SelectMany(row => row.Select(col => col)).ToLookup(n => n);
        var howManyMostDangerousPoints =
            overlaps
            .Where(o => o.Key >= 2)
            .Select(o => o.Key)
            .Sum(k => overlaps[k].Count());

        return howManyMostDangerousPoints;
    }

    public static async Task<object> One() => await GetOverlapsOverTwo(true);
    public static async Task<object> Two() => await GetOverlapsOverTwo(false);
}
