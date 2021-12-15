using LanguageExt;
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
    static Position[][] ToPositions(this int[][] input, int size)
    {
        var w = input.Length;
        var h = input[0].Length;
        var groupedByRow =
            Enumerable.Range(0, w).SelectMany(y =>
            Enumerable.Range(0, h).SelectMany(x =>
            {
                var positions = new List<Position>();
                var risk = input[y][x];
                for (int dY = 0; dY < size; dY++)
                {
                    for (int dX = 0; dX < size; dX++)
                    {
                        var subRisk = (risk + (dX + dY));
                        if(subRisk > 9) subRisk = subRisk - 9;
                        var subX = x + w * dX;
                        var subY = y + h * dY;
                        var pos = new Position(w*size, h*size, subX, subY, subRisk);
                        positions.Add(pos);
                    }
                }
                return positions;
            })).ToLookup(o => o.Y);
        var rows = groupedByRow.Select(X => X.Key).OrderBy(x => x);
        var cols = rows.Select(r => groupedByRow[r].OrderBy(x => x.X).ToArray()).ToArray();    
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
        current.GetLeft(positions).IfSome(p => neighbours.Add(p));
        foreach (var n in neighbours)
        {
            yield return n;
        }
    }

    static int Heuristic(Position end, Position current) => 
        end.X - current.X + end.Y - current.Y;

    static int CalcRisk(Position[][] positions, Position start, Position end)
    {
        var queue = new System.Collections.Generic.HashSet<Position>();        
        var cost = positions.SelectMany(p => p).ToDictionary(p => p, p => int.MaxValue);
        var score = positions.SelectMany(p => p).ToDictionary(p => p, p => int.MaxValue);

        queue.Add(start);
        cost[start] = 0;
        score[start] = Heuristic(end, start);

        while (queue.Count > 0)
        {
            var node = queue.OrderBy(q => score[q]).First();                        
            if (node == end) return cost[end];
            _ = queue.Remove(node);
            foreach (var neighbour in positions.GetNeighbours(node).OrderBy(x => x.Risk).AsParallel())
            {
                var newCost = cost[node] + neighbour.Risk;
                int oldCost = cost[neighbour];
                if (newCost >= oldCost) continue;
                cost[neighbour] = newCost;

                var new_score = newCost + Heuristic(end, neighbour);
                score[neighbour] = new_score;

                if (!queue.Contains(neighbour)) queue.Add(neighbour);
            }
        }        
        return -1;
    }

    private static async Task<object> Astar(int size)
    {
        var input = await GetInput();
        var positions = input.ToPositions(size);
        var start = positions.GetStart();
        var end = positions.GetEnd();
        var lowestRisk = CalcRisk(positions, start, end);
        return lowestRisk;
    }

    public static async Task<object> One() => await Astar(1);
    public static async Task<object> Two() => await Astar(5);
}