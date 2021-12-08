using System.Reflection;
using WinstonPuckett.PipeExtensions;


public static class Day08
{
    static IEnumerable<string[][]> GroupedByTwo(IEnumerable<string> data)
    {
        var group = data
            .Select((x, i) => (value: x.Replace("|", "").Trim(), index: i))
            .ToLookup(x => x.index / 2, x => x.value);
        foreach (var key in group.Select(g => g.Key))
        {
            yield return group[key].Select(g => g.Split(" ").ToArray()).ToArray();
        }
    }


    private static async Task<string[][][]> GetInput() =>
        (await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text.Split(" | ").Select(t => t.Trim().Split(" ").Select(c => c.Trim()).ToArray()).ToArray())
        .ToListAsync())
        .ToArray();

    public static async Task<object> One()
    {
        var input= await GetInput();
        var onlyOutputs = input.SelectMany(i => i[1]).ToArray();
        var ones = onlyOutputs.Count(o => o.Length == 2);
        var fours = onlyOutputs.Count(o => o.Length == 4);
        var sevens = onlyOutputs.Count(o => o.Length == 3);
        var eights = onlyOutputs.Count(o => o.Length == 7);
        var sum = ones + fours + sevens + eights;
        return sum;
    }

    public static async Task<object> Two()
    {
        var inputs = await GetInput();
        var sum = 0;

        foreach (var input in inputs)
        {
            var numbers = input.SelectMany(x => x.Select(c => c.ToArray())).ToArray();
            var output = input.Last();

            var ones = numbers.First(o => o.Length == 2).ToArray();
            var fours = numbers.First(o => o.Length == 4).ToArray();
            var sevens = numbers.First(o => o.Length == 3).ToArray();
            var eights = numbers.First(o => o.Length == 7).ToArray();

            var nines = numbers.First(x =>
                x.Length == 6 &&
                x.Except(sevens).Except(fours).Count() == 1)
                .ToArray();

            var sixes = numbers.First(x =>
                x.Length == 6 &&
                NotEq(x, nines) &&
                ones.Except(x).Count() == 1).ToArray();

            var zeros = numbers.First(x =>
                x.Length == 6 &&
                NotEq(x, nines) &&
                NotEq(x, sixes)).ToArray();

            var bottomLeft = eights.Except(nines).Single();
            var topRight = eights.Except(sixes).Single();
            var bottomRight = ones.Except(new[] { topRight }).Single();
            var middle = eights.Except(zeros).Single();
            var top = sevens.Except(new char[] { topRight, bottomRight }).Single();
            var topLeft = fours.Except(new[] { middle, topRight, bottomRight }).Single();
            var bottom = zeros.Except(new[] { top, topRight, topLeft, bottomLeft, bottomRight }).Single();

            var twos = new[] { top, topRight, middle, bottomLeft, bottom };
            var fives = new[] { top, topLeft, middle, bottomRight, bottom };
            var threes = new[] { top, topRight, middle, bottomRight, bottom };

            var map = new List<(char[] number, int value)>
            {
                (zeros, 0),
                (ones, 1),
                (twos, 2),
                (threes, 3),
                (fours, 4),
                (fives, 5),
                (sixes, 6),
                (sevens, 7),
                (eights, 8),
                (nines, 9),
            };
            var numbersDecoded = DecoreOutputWithMap(output, map);
            var value = ToInteger(numbersDecoded);
            sum += value;
        }
        return sum;

        static bool Eq(char[] x, char[] y) => x.All(i => y.Contains(i)) && x.Length == y.Length;
        static bool NotEq(char[] x, char[] y) => !Eq(x,y);

        static IEnumerable<string> DecoreOutputWithMap(string[] output, List<(char[] number, int value)> map) =>
                    output.Select(x => map.Single(m => Eq(x.ToArray(), m.number)).value.ToString());

        static int ToInteger(IEnumerable<string> numbersDecoded) => int.Parse(string.Join("", numbersDecoded));
    }
}