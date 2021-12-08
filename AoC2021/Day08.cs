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

    /*  Indexes:

         0000
        1    2
        1    2
         3333
        4    5
        4    5
         6666

     dddd
    e    a
    e    a
     ffff
    g    b
    g    b
     cccc

    acedgfb: 8
    cdfbe: 5
    gcdfa: 2
    fbcad: 3
    dab: 7
    cefabd: 9
    cdfgeb: 6
    eafb: 4
    cagedb: 0
    ab: 1
    */
    public static async Task<object> Two()
    {
        var inputs = await GetInput();
        var sum = 0;

        foreach (var input in inputs)
        {
            var numbers = input.SelectMany(x => x).ToArray();
            var output = input.Last();
            var ones = numbers.First(o => o.Length == 2).Select(c => c).ToArray();
            var fours = numbers.First(o => o.Length == 4).Select(c => c).ToArray();
            var sevens = numbers.First(o => o.Length == 3).Select(c => c).ToArray();
            var eights = numbers.First(o => o.Length == 7).Select(c => c).ToArray();

            var either2or5 = ones.Intersect(fours).ToArray();
            var either1or3 = fours.Except(either2or5).ToArray();
            var is0 = sevens.Except(ones).ToArray();
            var either4or6 = eights.Except(either1or3.Concat(either2or5).Concat(is0)).ToArray();

            int numbered = -1;

            foreach (var is4 in either4or6.Select(y => new[] { y }))
            {
                var is6 = either4or6.Except(is4).ToArray();
                var nines = eights.Except(is4).ToArray();

                var numbered2 = -1;
                foreach (var is2 in either2or5.Select(y => new[] { y }))
                {
                    var is5 = either2or5.Except(is2).ToArray();
                    var twos = nines.Except(is2).ToArray();
                    var is1 = eights.Except(twos).Except(is5).ToArray();
                    var threes = nines.Except(is1).ToArray();
                    var is3 = fours.Except(is1).Except(is2).Except(is5).ToArray();
                    var fives = nines.Except(is2).ToArray();
                    var sixes = fives.Concat(is4).ToArray();

                    var isSolution = is1.Any() && is3.Any();
                    if (isSolution)
                    {
                        var digitalOutputs = new List<int>();
                        char[][] outputsChars = output.Select(x => x.Select(y => y).ToArray()).ToArray();
                        foreach (var o in outputsChars)
                        {
                            int digitalOutput = -1;
                            if (o == ones) digitalOutput = 1;
                            if (o == twos) digitalOutput = 2;
                            if (o == threes) digitalOutput = 3;
                            if (o == fours) digitalOutput = 4;
                            if (o == fives) digitalOutput = 5;
                            if (o == sixes) digitalOutput = 6;
                            if (o == sevens) digitalOutput = 7;
                            if (o == eights) digitalOutput = 8;
                            digitalOutputs.Add(digitalOutput);
                        }

                        var stringed = string.Join("", digitalOutputs.Select(i => i.ToString()).ToArray());
                        numbered2 = int.Parse(stringed);
                        break;
                    }
                }
                if(numbered2 != -1 )
                {
                    numbered = numbered2;
                    break;
                }
            }
            sum += numbered;
        }
        return 1337;
    }
}
