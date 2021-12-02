public static class Day3
{
    private static async Task<IList<string>> GetInput() =>
        await Inputs
        .Read("inputs/day3.txt")
        .Select(text => text)
        .ToListAsync();

    public static async Task<int> One() => 1;
        //(await GetInput());

    public static async Task<int> Two() => 2;
        //(await GetInput());
}