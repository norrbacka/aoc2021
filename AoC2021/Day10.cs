using System.Linq;
using System.Reflection;

public static class Day10
{
    private static async Task<List<string>> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToListAsync();

    static char[] Openings = new[] { '{', '[', '(', '<' };
    static char[] Closures = new[] { '}', ']', ')', '>' };
    static Dictionary<char, char> ClosuresDict = new Dictionary<char, char>()
    {
        { '{', '}' },
        { '(', ')' },
        { '[', ']' },
        { '<', '>' }
    };
    static Dictionary<char, long> Polongs = new Dictionary<char, long>()
    {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 }
    };
    static Dictionary<char, long> AcPolongs = new Dictionary<char, long>()
    {
        { ')', 1 },
        { ']', 2 },
        { '}', 3 },
        { '>', 4 }
    };

    record Opening(long index, char symbol);

    static (char? illegal, Opening[] remaining) FindIllegal(char[] input, Opening[] openings, long i)
    {
        if (input.Length == i || !openings.Any())
        {
            return ((char?)null, openings);
        }

        var opening = openings.Last();
        var current = input[i];

        var newOpening = Openings.Contains(current) && Openings.Contains(opening.symbol);
        if (newOpening)
        {
            return FindIllegal(input, openings.Append(new Opening(i, current)).ToArray(), ++i);
        }

        var isClosing = Openings.Contains(opening.symbol) && ClosuresDict[opening.symbol] == current && opening.index > 0;
        if (isClosing)
        {
            var traverseToPreviousOpening = openings.Reverse().Skip(1).Reverse().ToArray();
            return FindIllegal(input, traverseToPreviousOpening, ++i);
        }

        var illegalClosing = opening.symbol != current && Closures.Contains(current) && Openings.Contains(opening.symbol);
        if (illegalClosing)
        {
            return (current, openings);
        }

        return ((char?)null, openings);
    }

    public static async Task<object> One()
    {
        var input = await GetInput();
        var firstInvalids = input.Select(line =>
        {
            var firstInvalid = FindIllegal(line.ToArray(), new[] { new Opening(0, line[0]) }, 1);
            return firstInvalid;
        }).ToList();
        var sum = firstInvalids.Where(x => x.illegal != null).Sum(c => Polongs[(char)c.illegal]);
        return sum;
    }

    public static async Task<object> Two()
    {
        var input = await GetInput();
        var firstInvalids = input.Select(line =>
        {
            var firstInvalid = FindIllegal(line.ToArray(), new[] { new Opening(0, line[0]) }, 1);
            return firstInvalid;
        }).ToList();
        var incomplete = firstInvalids.Where(x => x.illegal == null && x.remaining.Any()).ToList();
        var missing = incomplete.Select(inc => inc.remaining.Select(r => ClosuresDict[r.symbol]).Reverse().ToList()).ToList();
        var scores = missing.Select(m => m.Aggregate((long)0, (x, y) => (x * 5) + AcPolongs[y]));
        var score = scores.OrderBy(x => x).ToArray()[(int)Math.Ceiling(scores.Count() / 2m)-1];
        return score;
    }
}
