﻿public static class Day13
{
    private static async Task<List<string>> GetInput() =>
        await Inputs
        .Read("inputs/day13.txt")
        .Select(text => text)
        .ToListAsync();

    public static async Task<object> One() => await Task.Run(() => "Not yet implemented");
    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
