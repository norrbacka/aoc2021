using System.Reflection;

public static class Day11
{
    private static async Task<int[][]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(teXt => teXt.Select(i => int.Parse(i.ToString())).ToArray())
        .ToArrayAsync();

    record Octo(int RowCount, int ColCount, int X, int Y, int Energy)
    {
        public Octo SetEnergy(int energy) => 
            energy >= 0 && energy <= 10 ? 
                new Octo(RowCount, ColCount, X, Y, energy) : this;

        public bool Flashes => Energy > 9;

        public bool HasTop => Y > 0;
        public bool HasRight => X < (ColCount - 1);
        public bool HasBottom => Y < (RowCount - 1);
        public bool HasLeft => X > 0;
        public bool HasTopRight => HasTop && HasRight;
        public bool HasTopLeft => HasTop && HasLeft;
        public bool HasBottomRight => HasBottom && HasRight;
        public bool HasBottomLeft => HasBottom && HasLeft;

        public Octo GetTop(Octo[][] octos) => octos[Y - 1][X];
        public Octo GetTopRight(Octo[][] octos) => octos[Y - 1][X + 1];
        public Octo GetRight(Octo[][] octos) => octos[Y][X + 1];
        public Octo GetBottomRight(Octo[][] octos) => octos[Y + 1][X + 1];
        public Octo GetBottom(Octo[][] octos) => octos[Y + 1][X];
        public Octo GetBottomLeft(Octo[][] octos) => octos[Y + 1][X - 1];
        public Octo GetLeft(Octo[][] octos) => octos[Y][X - 1];
        public Octo GetTopLeft(Octo[][] octos) => octos[Y - 1][X - 1];
    };

    static Octo[][] ToOcto(this int[][] input) { 
        var octosList =
            Enumerable.Range(0, input.Length).SelectMany(y =>
            Enumerable.Range(0, input[y].Length).Select(x => 
                new Octo(input.Length, input[y].Length, x, y, input[y][x])))
            .ToLookup(o => o.Y);
        var rows = octosList.Select(X => X.Key);
        var cols = rows.Select(r => octosList[r].ToArray()).ToArray();
        return cols;
    }

    static (Octo[][], (int Y, int X)[]) Step(Octo[][] octos, Octo octo, (int Y, int X)[] flashed)
    {
        octo = octo.SetEnergy(octo.Energy + 1);
        octos[octo.Y][octo.X] = octo;
        if (octo.Flashes && !flashed.Contains((octo.Y, octo.X)))
        {
            flashed = flashed.Append((octo.Y, octo.X)).ToArray();
            if (octo.HasTop) (octos, flashed) = Step(octos, octo.GetTop(octos), flashed);
            if(octo.HasTopRight) (octos, flashed) = Step(octos, octo.GetTopRight(octos), flashed);
            if(octo.HasRight) (octos, flashed) = Step(octos, octo.GetRight(octos), flashed);
            if(octo.HasBottomRight) (octos, flashed) = Step(octos, octo.GetBottomRight(octos), flashed);
            if(octo.HasBottom) (octos, flashed) = Step(octos, octo.GetBottom(octos), flashed);
            if(octo.HasBottomLeft) (octos, flashed) = Step(octos, octo.GetBottomLeft(octos), flashed);
            if(octo.HasLeft) (octos, flashed) = Step(octos, octo.GetLeft(octos), flashed);
            if(octo.HasTopLeft) (octos, flashed) = Step(octos, octo.GetTopLeft(octos), flashed);
        }
        return (octos, flashed);
    }

    static (Octo[][] Octos, int TotalFlashes) Simulate(this Octo[][] octos, int step, int stop)
    {
        int totalFlashes = 0;
        do
        {
            var flashed = new (int Y, int X)[] { };
            for (int Y = 0; Y < octos.Length; Y++)
            {
                for (int X = 0; X < octos[Y].Length; X++)
                {
                    (octos, flashed) = Step(octos, octos[Y][X], flashed);
                }
            }
            totalFlashes += flashed.Count();
            foreach (var flash in flashed)
            {
                octos[flash.Y][flash.X] = octos[flash.Y][flash.X].SetEnergy(0);
            }
        }
        while (++step <= stop); 
        return (octos, totalFlashes);
    }

    public static async Task<object> One() =>
         (await GetInput())
        .ToOcto()
        .Simulate(1, 100)
        .TotalFlashes;

    static bool AllHasFlashed(this Octo[][] octos) => 
        octos.SelectMany(o => o).All(o => o.Energy == 0);

    static int SimulateUntilAllFlashes(this Octo[][] octos)
    {
        int step = 0;
        do
        {
            var flashed = Array.Empty<(int Y, int X)>();
            for (int Y = 0; Y < octos.Length; Y++)
            {
                for (int X = 0; X < octos[Y].Length; X++)
                {
                    (octos, flashed) = Step(octos, octos[Y][X], flashed);
                }
            }
            foreach (var flash in flashed)
            {
                octos[flash.Y][flash.X] = octos[flash.Y][flash.X].SetEnergy(0);
            }
            step++;
        }
        while (!octos.AllHasFlashed());
        return step;
    }

    public static async Task<object> Two() => 
        (await GetInput())
        .ToOcto()
        .SimulateUntilAllFlashes();
}