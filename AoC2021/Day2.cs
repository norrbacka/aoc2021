using static MoreLinq.Extensions.PairwiseExtension;
using WinstonPuckett.PipeExtensions;
using System.Reflection;

public static class Day2
{
    private static async Task<IList<(string Ins, int Unit)>> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => (Ins: text.Split(" ")[0].Trim().ToLower(), Unit: int.Parse(text.Split(" ")[1])))
        .ToListAsync();

    public static async Task<int> One() => 
        (await GetInput())
        .Select(i => (
            x: i.Ins switch { "forward" => i.Unit, _ => 0 },
            y: i.Ins switch { "down" => i.Unit, "up" => -i.Unit, _ => 0 }
        ))
        .Aggregate((p, n) => (
            x: p.x + n.x, 
            y: p.y + n.y
        ))
        .Pipe(p => p.x * p.y);

    public static async Task<int> Two() =>
        (await GetInput())
        .Select(i => (
            x: i.Ins switch { "forward" => i.Unit, _ => 0 },
            y: 0,
            aim: i.Ins switch { "down" => i.Unit, "up" => -i.Unit, _ => 0 }
        ))
        .Aggregate((p, n) => (
            x: p.x + n.x,
            y: p.y + (n.x * (p.aim + n.aim)),
            aim: p.aim + n.aim
        ))
        .Pipe(p => p.x * p.y);
}