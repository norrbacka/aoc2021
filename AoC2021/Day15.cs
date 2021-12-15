using LanguageExt;
using System.Linq;
using System.Reflection;

public static class Day15
{
    private static async Task<int[][]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(line => line.Select(c => int.Parse(c.ToString())).ToArray())
        .ToArrayAsync();

    record Position(int ColCount, int RowCount, int X, int Y, int Risk)
    {
        public bool HasTop => Y > 0;
        public bool HasRight => X < (ColCount - 1);
        public bool HasBottom => Y < (RowCount - 1);
        public bool HasLeft => X > 0;
        public bool HasTopRight => HasTop && HasRight;
        public bool HasTopLeft => HasTop && HasLeft;
        public bool HasBottomRight => HasBottom && HasRight;
        public bool HasBottomLeft => HasBottom && HasLeft;
        public Option<Position> GetTop(Position[][] Positions) => HasTop ? Positions[Y - 1][X] : Option<Position>.None;
        public Option<Position> GetTopRight(Position[][] Positions) => HasTopRight ? Positions[Y - 1][X + 1] : Option<Position>.None;
        public Option<Position> GetRight(Position[][] Positions) => HasRight ? Positions[Y][X + 1] : Option<Position>.None;
        public Option<Position> GetBottomRight(Position[][] Positions) => HasBottomRight ? Positions[Y + 1][X + 1] : Option<Position>.None;
        public Option<Position> GetBottom(Position[][] Positions) => HasBottom ? Positions[Y + 1][X] : Option<Position>.None;
        public Option<Position> GetBottomLeft(Position[][] Positions) => HasBottomLeft ? Positions[Y + 1][X - 1] : Option<Position>.None;
        public Option<Position> GetLeft(Position[][] Positions) => HasLeft ? Positions[Y][X - 1] : Option<Position>.None;
        public Option<Position> GetTopLeft(Position[][] Positions) => HasTopLeft ? Positions[Y - 1][X - 1] : Option<Position>.None;
    }
    static Position[][] ToPositions(this int[][] input)
    {
        var groupedByRow =
            Enumerable.Range(0, input.Length).SelectMany(y =>
            Enumerable.Range(0, input[y].Length).Select(x =>
                new Position(input.Length, input[y].Length, x, y, input[y][x]))).ToLookup(o => o.Y);
        var rows = groupedByRow.Select(X => X.Key);
        var cols = rows.Select(r => groupedByRow[r].ToArray()).ToArray();
        return cols;
    }

    static Position GetStart(this Position[][] positions) => positions[0][0];
    static Position GetEnd(this Position[][] positions) => positions[positions.Length - 1][positions[0].Length - 1];

    static IEnumerable<Position> GetNeighbours(this Position[][] positions, Position current)
    {
        var neighbours = new List<Position>();
        current.GetTop(positions).IfSome(p => neighbours.Add(p));
        current.GetRight(positions).IfSome(p => neighbours.Add(p));
        current.GetBottom(positions).IfSome(p => neighbours.Add(p));
        current.GetBottom(positions).IfSome(p => neighbours.Add(p));
        foreach (var n in neighbours)
        {
            yield return n;
        }
    }

    /*
     * procedure BFS(G, root) is
 2      let Q be a queue
 3      label root as explored
 4      Q.enqueue(root)
 5      while Q is not empty do
 6          v := Q.dequeue()
 7          if v is the goal then
 8              return v
 9          for all edges from v to w in G.adjacentEdges(v) do
10              if w is not labeled as explored then
11                  label w as explored
12                  Q.enqueue(w)
    */

    static int H(Position end, Position current) =>
        current.Risk * (Math.Abs(current.X - end.X) + Math.Abs(current.Y - end.Y));



    static Position[] Search(Position[][] positions, Position start, Position end, Position[][] allPaths)
    {
        if(allPaths.Count() == 3)
        {
            return allPaths.OrderBy(x => x.Sum(y => y.Risk)).First();
        }
        var openSet = new PriorityQueue<Position, int>();
        openSet.Enqueue(start, 0);

        var cameFrom = new Dictionary<Position, Position>();
        var gScore = positions.SelectMany(p => p).ToDictionary(p => p, p => int.MaxValue);
        var fScore = new Dictionary<Position, int>()
        {
            { start, 0}
        };
        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == end)
            {
                var fullPath = GetPath(start, cameFrom, current);
                allPaths = allPaths.Append(fullPath).ToArray();
                return Search(positions, start, end, allPaths); //TODO: reverse to previous decision and continue
            }

            var neighbours = positions.GetNeighbours(current);
            foreach (var n in neighbours)
            {
                var score = H(end, n);
                if (score < gScore[n])
                {
                    cameFrom[n] = current;
                    gScore[n] = score;
                    if (!openSet.UnorderedItems.Contains((n, score)))
                    {
                        openSet.Enqueue(n, score);
                    }
                }
            }
        }
        return new Position[] {};

        //static bool CoveredAlready(Position[][] allPaths, Position start, Dictionary<Position, Position> cameFrom, Position current)
        //{        
        //    var bar = trimedToSameLength.Any(t => 
        //        string.Join("", t.Select(u => $"{u.X}{u.Y}")) ==
        //        string.Join("", currentPath.Select(u => $"{u.X}{u.Y}")));
        //    if(bar)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        static Position[] GetPath(Position start, Dictionary<Position, Position> cameFrom, Position current)
        {
            var foo = current;
            var totalPath = new Position[] { foo };
            do
            {
                foo = cameFrom[foo];
                totalPath = totalPath.Prepend(foo).ToArray();
            } while (foo != start);
            return totalPath;
        }
    }

    public static async Task<object> One()
    {
        var input = await GetInput();
        var positions = input.ToPositions();
        var start = positions.GetStart();
        var end = positions.GetEnd();
        var lowestRisk = Search(positions, start, end, new Position[][] { });
        var stringified = string.Join(" -> ", lowestRisk.Select(x => $"({x.X}, {x.Y}, {x.Risk})"));
        Console.WriteLine(stringified);
        var currentRisk = lowestRisk.Sum(s => s.Risk);
        return $"Current risk: {currentRisk}";
    }
    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
