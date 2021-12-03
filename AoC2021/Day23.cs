public static class Day23
{
    private static async Task<List<string>> GetInput() =>
        await Inputs
        .Read("inputs/day23.txt")
        .Select(text => text)
        .ToListAsync();

    public static async Task<object> One() => "foo";
    public static async Task<object> Two() => "bar";
}
