public static class Day21
{
    private static async Task<List<string>> GetInput() =>
        await Inputs
        .Read("inputs/day21.txt")
        .Select(text => text)
        .ToListAsync();

    public static async Task<object> One() => "foo";
    public static async Task<object> Two() => "bar";
}
