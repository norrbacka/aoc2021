using System.Diagnostics;
using System.Reflection;

public static class Day11
{
    private static async Task<int[][]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Select(i => int.Parse(i.ToString())).ToArray())
        .ToArrayAsync();

    record Octo(int rowLength, int colLength, int x, int y, int energy)
    {
        public Octo SetEnergy(int e) => e >= 0 && e <= 10 ? new Octo(rowLength, colLength, x, y, e) : this;

        public bool Flashes => energy > 9;

        public bool HasTop => y > 0;
        public bool HasRight => x < (colLength - 1);
        public bool HasBottom => y < (rowLength - 1);
        public bool HasLeft => x > 0;
        public bool HasTopRight => HasTop && HasRight;
        public bool HasTopLeft => HasTop && HasLeft;
        public bool HasBottomRight => HasBottom && HasRight;
        public bool HasBottomLeft => HasBottom && HasLeft;

        public Octo GetTop(Octo[][] octos) => octos[y - 1][x];
        public Octo GetTopRight(Octo[][] octos) => octos[y - 1][x + 1];
        public Octo GetRight(Octo[][] octos) => octos[y][x + 1];
        public Octo GetBottomRight(Octo[][] octos) => octos[y + 1][x + 1];
        public Octo GetBottom(Octo[][] octos) => octos[y + 1][x];
        public Octo GetBottomLeft(Octo[][] octos) => octos[y + 1][x - 1];
        public Octo GetLeft(Octo[][] octos) => octos[y][x - 1];
        public Octo GetTopLeft(Octo[][] octos) => octos[y - 1][x - 1];
    };

    static Octo[][] ToOcto(this int[][] input) { 
        var octosList =
            Enumerable.Range(0, input.Length).SelectMany(y =>
            Enumerable.Range(0, input[y].Length).Select(x => 
                new Octo(input.Length, input[y].Length, x, y, input[y][x])))
            .ToLookup(o => o.y);
        var rows = octosList.Select(x => x.Key);
        var cols = rows.Select(r => octosList[r].ToArray()).ToArray();
        return cols;
    }

    static (Octo[][], (int y, int x)[]) Step(Octo[][] octos, Octo octo, (int y, int x)[] flashed)
    {
        octo = octo.SetEnergy(octo.energy + 1);
        octos[octo.y][octo.x] = octo;
        if (octo.Flashes && !flashed.Contains((octo.y, octo.x)))
        {
            flashed = flashed.Append((octo.y, octo.x)).ToArray();
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
        octos.Print(0);
        do
        {
            var flashed = new (int y, int x)[] { };
            for (int y = 0; y < octos.Length; y++)
            {
                for (int x = 0; x < octos[y].Length; x++)
                {
                    (octos, flashed) = Step(octos, octos[y][x], flashed);
                }
            }
            totalFlashes += flashed.Count();
            foreach (var flash in flashed)
            {
                octos[flash.y][flash.x] = octos[flash.y][flash.x].SetEnergy(0);
            }
            octos.Print(step);
        }
        while (++step <= stop); 
        return (octos, totalFlashes);
    }

    static void Print(this Octo[][] octos, int step)
    {
        Debug.WriteLine(step == 0 ? "Before any steps:" : $"After step {step}:");
        for (int y = 0; y < octos.Length; y++)
        {
            var numbers = string.Join("", octos[y].Select(o => o.energy));
            Debug.WriteLine(numbers);
        }
        Debug.WriteLine("");
    }

    public static async Task<object> One() =>
         (await GetInput())
        .ToOcto()
        .Simulate(1, 100)
        .TotalFlashes;

    static bool AllHasFlashed(this Octo[][] octos) => 
        octos.SelectMany(o => o).All(o => o.energy == 0);

    static int SimulateUntilAllFlashes(this Octo[][] octos)
    {
        int step = 0;
        do
        {
            var flashed = new (int y, int x)[] { };
            for (int y = 0; y < octos.Length; y++)
            {
                for (int x = 0; x < octos[y].Length; x++)
                {
                    (octos, flashed) = Step(octos, octos[y][x], flashed);
                }
            }
            foreach (var flash in flashed)
            {
                octos[flash.y][flash.x] = octos[flash.y][flash.x].SetEnergy(0);
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