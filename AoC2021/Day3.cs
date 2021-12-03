using WinstonPuckett.PipeExtensions;

public static class Day3
{
    private static async Task<List<bool[]>> GetInput() =>
        await Inputs
        .Read("inputs/day3.txt")
        .Select(text => text.Trim().Select(c => c == '1').ToArray())
        .ToListAsync(); 

    public static async Task<object> One() =>
         (await GetInput())
        .Pipe(ComputePowerConsumption);

    public static decimal ComputePowerConsumption(IEnumerable<bool[]> inputs) =>
        (inputs
            .Pipe(Flip)
            .Pipe(GetMostCommonBytes)
            .Pipe(ToDecimal)) 
        *
        (inputs
            .Pipe(Flip)
            .Pipe(GetLeastCommonBytes)
            .Pipe(ToDecimal));

    public static IEnumerable<bool[]> Flip(IEnumerable<bool[]> original) =>
        Enumerable.Range(0, original.First().Count())
        .Select(i => original.Select(o => o[i]).ToArray());

    public static bool[] GetMostCommonBytes(IEnumerable<bool[]> inputs) => 
        inputs.Select(GetMostCommonBit).ToArray();

    public static bool GetMostCommonBit(bool[] inputs) => 
        inputs.Count(d => d) >= inputs.Count(d => !d);

    public static bool[] GetLeastCommonBytes(IEnumerable<bool[]> inputs) => 
        inputs.Select(GetLeastCommonBit).ToArray();

    public static bool GetLeastCommonBit(bool[] inputs) => 
        inputs.Count(d => d) < inputs.Count(d => !d);

    private static string ToByteString(bool[] inputs) =>
        string.Join("", inputs.Select(i => i ? "1" : "0"));

    private static decimal ToDecimal(bool[] inputs) =>
        Convert.ToInt32(inputs.Pipe(ToByteString),2);

    public static async Task<object> Two() =>
         (await GetInput())
        .Pipe(ComputeLifeSupport);

    public static decimal ComputeLifeSupport(IEnumerable<bool[]> inputs) =>
        GetOxygenGeneratorRating(inputs) * GetCo2ScrubberRating(inputs);

    public static decimal GetOxygenGeneratorRating(IEnumerable<bool[]> inputs, int index = 0) =>
        Traverse(inputs, (i, x) => i.ElementAt(x), (i, x) => !i.ElementAt(x));

    public static decimal GetCo2ScrubberRating(IEnumerable<bool[]> inputs, int index = 0) =>
        Traverse(inputs, (i, x) => !i.ElementAt(x), (i, x) => i.ElementAt(x));

    public static decimal Traverse(
        IEnumerable<bool[]> inputs, 
        Func<IEnumerable<bool>, int, bool> MoreOnesOrEqFilter, 
        Func<IEnumerable<bool>, int, bool> MoreZerosFilter,
        int index = 0)
    {
        if (inputs.Count() == 1)
        {
            return ToDecimal(inputs.First());
        }
        var input = Flip(inputs).ElementAt(index);
        var moreOnesOrEq = input.Count(i => i) >= input.Count(i => !i);
        var rest = inputs.Where(x => moreOnesOrEq ? MoreOnesOrEqFilter(x, index) : MoreZerosFilter(x, index));
        return Traverse(rest, MoreOnesOrEqFilter, MoreZerosFilter, index + 1);
    }
}