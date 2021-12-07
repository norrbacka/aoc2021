using LanguageExt;
using static MoreLinq.Extensions.PairwiseExtension;
using WinstonPuckett.PipeExtensions;
using Microsoft.FSharp.Collections;
using System.Reflection;

public static class Day1
{
    private static async Task<IList<int>> GetInput() => 
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(i => int.Parse(i))
        .ToListAsync();

    public static async Task<int> One() =>
        (await GetInput())
        .Pipe(CountIncreases);

    public static async Task<int> Two() =>
        (await GetInput())
        .Pipe(ToGroupsOfThree)
        .Pipe(ToSums)
        .Pipe(CountIncreases);

    public static int CountIncreases(IList<int> inputs) => 
        inputs
        .Prepend(int.MaxValue)
        .Pairwise((previous, current) => current > previous ? 1 : 0)
        .Sum();

    public static IEnumerable<IList<int>> ToGroupsOfThree(IList<int> inputs) => 
        SeqModule
        .Windowed(3, inputs)
        .Select(x => x.ToList());

    public static IList<int> ToSums(IEnumerable<IList<int>> inputsGrouped) =>
        inputsGrouped
        .Select(i => i.Sum())
        .ToList();
}