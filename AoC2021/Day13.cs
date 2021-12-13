using System.Diagnostics;
using System.Reflection;
using System.Linq;
using WinstonPuckett.PipeExtensions;

public static class Day13
{
    static async Task<string[]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToArrayAsync();

    static (int x, int y)[] ToFilledCoordinates(this string[] inputs) =>
        inputs
        .Where(x => !x.Contains("fold") && !string.IsNullOrWhiteSpace(x))
        .Select(text =>
        {
            var split = text.Split(",");
            var x = int.Parse(split[0]);
            var y = int.Parse(split[1]);
            return (x, y);
        })
        .ToArray();

    static (char axis, int foldValue)[] ToFoldInstructions(this string[] inputs) =>
        inputs
        .Where(x => x.Contains("fold"))
        .Select(text =>
        {
            var split = text.Split("along ").Last().Trim().Split("=");
            var axis = char.Parse(split[0]);
            var foldValue= int.Parse(split[1]);
            return (axis, foldValue);
        })
        .ToArray();

    static ((int x, int y)[] filled, (char axis, int foldValue)[] instructions) ParseInput(this string[] inputs) =>
        (inputs.ToFilledCoordinates(), inputs.ToFoldInstructions());

    static (int x, int y)[] Fold(this (int x, int y)[] dots, char direction, int axisValue) =>
        direction switch
        {
            'x' => dots.Select(d => (axisValue - Math.Abs(d.x - axisValue), d.y)).Distinct().ToArray(),
            'y' => dots.Select(d => (d.x, axisValue - Math.Abs(d.y - axisValue))).Distinct().ToArray(),
            _ => throw new NotImplementedException()
        };

    private static string Print(this (int x, int y)[] filled) =>
        Enumerable.Range(0, filled.Max(x => x.y + 1))
        .Select(y =>
        Enumerable.Range(0, filled.Max(x => x.x + 1))
        .Select(x =>
        filled.Contains((x, y)) ? "#" : "."))
        .Select(line => string.Join("", line))
        .Pipe(lines => string.Join(Environment.NewLine, lines));

    public static async Task<object> One() =>
        (await GetInput())
        .ParseInput()
        .Pipe((filled, instructions) => filled.Fold(instructions[0].axis, instructions[0].foldValue))
        .Count();

    public static async Task<object> Two() =>
        (await GetInput())
        .ParseInput()
        .Pipe((filled, instructions) => 
            instructions.Aggregate(filled, (filled, instruction) => 
                filled.Fold(instruction.axis, instruction.foldValue)))
        .Print();
}