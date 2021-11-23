public static class Day1
{
    public static async Task<string> Run() =>
        (await Inputs
        .Read("inputs/day1.txt")
        .SumAsync(row => 1)
        ).ToString();
}