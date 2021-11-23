public static class Test
{
    public static async Task<string> Run() => 
        (await Inputs
        .Read("inputs/test.txt") //AoC 2019.1
        .SumAsync(row => Math.Floor(decimal.Parse(row)/3.0m)-2.0m)
        ).ToString();           
}
