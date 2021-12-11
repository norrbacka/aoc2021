using LanguageExt;
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
        public Octo SetEnergy(int energy) => new(RowCount, ColCount, X, Y, energy);
        public bool Flashes => Energy > 9;
        public bool HasTop => Y > 0;
        public bool HasRight => X < (ColCount - 1);
        public bool HasBottom => Y < (RowCount - 1);
        public bool HasLeft => X > 0;
        public bool HasTopRight => HasTop && HasRight;
        public bool HasTopLeft => HasTop && HasLeft;
        public bool HasBottomRight => HasBottom && HasRight;
        public bool HasBottomLeft => HasBottom && HasLeft;
        public Option<Octo> GetTop(Octo[][] octos) => HasTop ? octos[Y - 1][X] : Option<Octo>.None;
        public Option<Octo> GetTopRight(Octo[][] octos) => HasTopRight ? octos[Y - 1][X + 1] : Option<Octo>.None;
        public Option<Octo> GetRight(Octo[][] octos) => HasRight ? octos[Y][X + 1]: Option<Octo>.None;
        public Option<Octo> GetBottomRight(Octo[][] octos) => HasBottomRight ? octos[Y + 1][X + 1] : Option<Octo>.None;
        public Option<Octo> GetBottom(Octo[][] octos) => HasBottom ? octos[Y + 1][X] : Option<Octo>.None;
        public Option<Octo> GetBottomLeft(Octo[][] octos) =>  HasBottomLeft ? octos[Y + 1][X - 1] : Option<Octo>.None;
        public Option<Octo> GetLeft(Octo[][] octos) => HasLeft ? octos[Y][X - 1] : Option<Octo>.None;
        public Option<Octo> GetTopLeft(Octo[][] octos) => HasTopLeft ? octos[Y - 1][X - 1] : Option<Octo>.None;
    };

    static Octo[][] ToOcto(this int[][] input) { 
        var octoGroupedByRow =
            Enumerable.Range(0, input.Length).SelectMany(y =>
            Enumerable.Range(0, input[y].Length).Select(x => 
                new Octo(input.Length, input[y].Length, x, y, input[y][x]))).ToLookup(o => o.Y);
        var rows = octoGroupedByRow.Select(X => X.Key);
        var cols = rows.Select(r => octoGroupedByRow[r].ToArray()).ToArray();
        return cols;
    }

    static (Octo[][], (int Y, int X)[]) Step(Octo[][] octos, Octo octo, (int Y, int X)[] flashed)
    {
        octo = octo.SetEnergy(octo.Energy + 1);
        octos[octo.Y][octo.X] = octo;
        if (octo.Flashes && !flashed.Contains((octo.Y, octo.X)))
        {
            flashed = flashed.Append((octo.Y, octo.X)).ToArray();
            (octos, flashed) = octo.GetTop(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
            (octos, flashed) = octo.GetTopRight(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
            (octos, flashed) = octo.GetRight(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
            (octos, flashed) = octo.GetBottomRight(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
            (octos, flashed) = octo.GetBottom(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
            (octos, flashed) = octo.GetBottomLeft(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
            (octos, flashed) = octo.GetLeft(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
            (octos, flashed) = octo.GetTopLeft(octos).Match(octo => Step(octos, octo, flashed), () => (octos, flashed));
        }
        return (octos, flashed);
    }

    static (int Y, int X)[] SimulateNext(ref Octo[][] octos)
    {
        var flashed = Array.Empty<(int Y, int X)>();
        for (int Y = 0; Y < octos.Length; Y++)
            for (int X = 0; X < octos[Y].Length; X++)
                (octos, flashed) = Step(octos, octos[Y][X], flashed);
        foreach (var (Y, X) in flashed)
            octos[Y][X] = octos[Y][X].SetEnergy(0);
        return flashed;
    }

    static (Octo[][] Octos, int TotalFlashes) Simulate(this Octo[][] octos, int step, int stop)
    {
        int totalFlashes = 0;
        do { totalFlashes += SimulateNext(ref octos).Length; } while (++step <= stop); 
        return (octos, totalFlashes);
    }

    public static async Task<object> One() =>
         (await GetInput())
        .ToOcto()
        .Simulate(1, 100)
        .TotalFlashes;

    static bool AllHasFlashed(this Octo[][] octos) => octos.SelectMany(o => o).All(o => o.Energy == 0);

    static int SimulateUntilAllFlashes(this Octo[][] octos)
    {
        int step = 0;
        do { _ = SimulateNext(ref octos); step++; } while (!octos.AllHasFlashed());
        return step;
    }

    public static async Task<object> Two() => 
        (await GetInput())
        .ToOcto()
        .SimulateUntilAllFlashes();
}