using WinstonPuckett.PipeExtensions;

public static class Day6
{
    private static async Task<long[]> GetInput() =>
        (await Inputs
        .Read("inputs/day6.txt")
        .Select(text => text.Split(",").Select(s => long.Parse(s)))
        .ToListAsync())
        .SelectMany(x => x)
        .ToArray();

    public static long[] ToCountedInput(long[] input) => new long[]
    {
        input.Count(i => i == 0),
        input.Count(i => i == 1),
        input.Count(i => i == 2),
        input.Count(i => i == 3),
        input.Count(i => i == 4),
        input.Count(i => i == 5),
        input.Count(i => i == 6),
        input.Count(i => i == 7),
        input.Count(i => i == 8)
    };

    public static long[] ProceedOneDay(long[] input) => new long[]
    {
        input[1],
        input[2],
        input[3],
        input[4],
        input[5],
        input[6],
        input[7] + input[0],
        input[8],
        input[0],
    };

    public static long[] SimulateDays(long[] input, int days, int currentDay = 1) =>
        currentDay > days ? 
            input : 
            SimulateDays(input = ProceedOneDay(input), days, ++currentDay);

    public static async Task<object> One() =>
        (await GetInput())
        .Pipe(ToCountedInput)
        .Pipe(i => SimulateDays(i, 80))
        .Sum();

    public static async Task<object> Two() => 
        (await GetInput())
        .Pipe(ToCountedInput)
        .Pipe(i => SimulateDays(i, 256))
        .Sum();
}