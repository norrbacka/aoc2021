using LanguageExt;

public static class Day7
{
    private static async Task<int[]> GetInput() =>
        (await Inputs
        .Read("inputs/day7.txt")
        .Select(text => text.Split(",").Select(s => int.Parse(s)))
        .ToListAsync())
        .SelectMany(x => x)
        .ToArray();

    static IEnumerable<int> GetPossibleEndPositions(int[] inputs)
    {
        var max = inputs.Max();
        for(var min = inputs.Min(); min <= max; min++)
        {
            yield return min;
        }
    }

    static int CalculateFuel(int currentFuelConsumption, int number, int horizontalPos) =>
        currentFuelConsumption + Math.Abs(number - horizontalPos);

    public static async Task<object> One()
    {
        var input = await GetInput();
        (int horizontalPos, int totalFuelConsumption) bestFuelPos = (-1, int.MaxValue);   
        foreach(var horizonalPos in GetPossibleEndPositions(input))
        {
            var fuelConsumption = 0;
            bool skiped = false;
            foreach (var number in input)
            {
                fuelConsumption = CalculateFuel(fuelConsumption, number, horizonalPos);
                if(fuelConsumption > bestFuelPos.totalFuelConsumption)
                {
                    skiped = true;
                    continue;
                }
            }
            if(!skiped)
            {
                bestFuelPos = (horizonalPos, fuelConsumption);
            }
        }
        Console.WriteLine($"horizontalPos: {bestFuelPos.horizontalPos}, totalFuelConsumption: {bestFuelPos.totalFuelConsumption}");
        return bestFuelPos.totalFuelConsumption;
    }

    static int CalculateFuelWithSteps(int currentFuelConsumption, int number, int horizontalPos)
    {
        var steps = Math.Abs(number - horizontalPos);
        var totalCost = Enumerable.Range(1, steps).Sum();
        return currentFuelConsumption + totalCost;
    }

    public static async Task<object> Two()
    {
        var input = await GetInput();
        (int horizontalPos, int totalFuelConsumption) bestFuelPos = (-1, int.MaxValue);
        foreach (var horizonalPos in GetPossibleEndPositions(input))
        {
            var fuelConsumption = 0;
            bool skiped = false;
            foreach (var number in input)
            {
                fuelConsumption = CalculateFuelWithSteps(fuelConsumption, number, horizonalPos);
                if (fuelConsumption > bestFuelPos.totalFuelConsumption)
                {
                    skiped = true;
                    continue;
                }
            }
            if (!skiped)
            {
                bestFuelPos = (horizonalPos, fuelConsumption);
            }
        }
        Console.WriteLine($"horizontalPos: {bestFuelPos.horizontalPos}, totalFuelConsumption: {bestFuelPos.totalFuelConsumption}");
        return bestFuelPos.totalFuelConsumption;
    }
}
