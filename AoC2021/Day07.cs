using LanguageExt;
using System.Reflection;
using WinstonPuckett.PipeExtensions;
public static class Day07
{
    private static async Task<int[]> GetInput() =>
        (await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Split(",").Select(s => int.Parse(s)))
        .ToListAsync())
        .SelectMany(x => x)
        .ToArray();

    static IEnumerable<int> GetPossibleEndPositions(int[] inputs)
    {
        for(var min = inputs.Min(); min <= inputs.Max(); min++) yield return min;
    }

    static int CalculateFuel(int currentFuelConsumption, int number, int horizontalPos) =>
        currentFuelConsumption + Math.Abs(number - horizontalPos);

    static async Task<(int horizontalPos, int totalFuelConsumption)> CalculateLeastFuel(Func<int, int, int, int> calculateFuelFunc)
    {
        var input = await GetInput();
        (int horizontalPos, int totalFuelConsumption) bestFuelPos = (-1, int.MaxValue);
        foreach (var horizonalPos in GetPossibleEndPositions(input))
        {
            var newFuelResult = true; var fuelConsumption = 0;
            foreach (var number in input)
            {
                fuelConsumption = calculateFuelFunc(fuelConsumption, number, horizonalPos);
                newFuelResult = fuelConsumption <= bestFuelPos.totalFuelConsumption;
                if (newFuelResult) continue;
            }
            if (newFuelResult) bestFuelPos = (horizonalPos, fuelConsumption);
        }
        return bestFuelPos;
    }

    public static async Task<object> One() =>
        (await CalculateLeastFuel(CalculateFuel))
        .totalFuelConsumption;

    static int CalculateFuelWithSteps(int currentFuelConsumption, int number, int horizontalPos) => 
        Math.Abs(number - horizontalPos)
        .Pipe(steps => Enumerable.Range(1, steps).Sum())
        .Pipe(totalCost => currentFuelConsumption + totalCost);

    public static async Task<object> Two() =>
        (await CalculateLeastFuel(CalculateFuelWithSteps))
        .totalFuelConsumption;
}