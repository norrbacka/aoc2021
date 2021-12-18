using MoreLinq;
using System.Reflection;

public static class Day16
{
    private static async Task<string[]> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToArrayAsync();

    static string HexToBits(this string c) => c switch
    {
        "0" => "0000",
        "1" => "0001",
        "2" => "0010",
        "3" => "0011",
        "4" => "0100",
        "5" => "0101",
        "6" => "0110",
        "7" => "0111",
        "8" => "1000",
        "9" => "1001",
        "A" => "1010",
        "B" => "1011",
        "C" => "1100",
        "D" => "1101",
        "E" => "1110",
        "F" => "1111",
        _ => throw new Exception()
    };
    static string HexToBits(this char c) => c.ToString().HexToBits();
    static int ToInt(this string text) => Convert.ToInt32(text, 2);
    static string Join<T>(this IEnumerable<T> array) => string.Join("", array);
    static int ToInt<T>(this IEnumerable<T> array) => array.Join().ToInt();
    static (string Taken, string Remainder) Fetch(this string text, int toTake) =>
        (text.Take(toTake).Join(), text.Skip(toTake).Join());

    static string Header(this string text) => text.Take(6).Join();
    static int PacketVersion(this string text) => text.Take(3).ToInt();
    static int TypeId(this string text) => text.Skip(3).Take(3).ToInt();
    static int LengthTypeId(this string text) => text.Skip(6).Take(1).ToInt();


    static decimal LiteralValue(this string text)
    {
        var data = text.Skip(6).Join();
        var groups = data
            .Batch(5)
            .Where(x => x.Count() == 5)
            .TakeUntil(x => x.First() == '0')
            .Select(x => x.Skip(1).Join());
        var toDecimal = groups.ToInt();
        return toDecimal;
    }

    record Package(string Input)
    {
        public int Version => Input.Header().PacketVersion();
        public int TypeId => Input.Header().TypeId();
        public bool IsLiteral => TypeId == 4;
        public bool IsOperator => !IsLiteral;
        public string GetRemainder()
        {
            return string.Empty;
        }
    }
    record Literal : Package
    {
        public Literal(string Input) : base(Input)
        {
            LiteralValue = Input.LiteralValue();
        }
        public decimal LiteralValue { get; }
    }
    record Operator : Package
    {
        public Operator(string Input) : base(Input)
        {
        }

        public int LengthTypeId => Input.LengthTypeId();

        public string Body => Input.Skip(7).Join();
    }

    static Package Parse(string input)
    {
        var p = new Package(input);
        if (p.IsLiteral)
        {
            return new Literal(input);
        }
        if(p.IsOperator)
        {
            return new Operator(input);
        }
        throw new Exception("Neither a literla or operator!");
    }



    static IEnumerable<Package> ParseRecur(Package package, IEnumerable<Package> packages)
    {
        if (package is Literal l)
        {
            packages = MoreEnumerable.Append(packages, tail: l); 
        }
        if (package is Operator o)
        {
          
            packages = MoreEnumerable.Append(packages, tail: o);
            if(o.LengthTypeId == 0)
            {
                var length = o.Body.Take(15).ToInt();
                var bits = o.Body.Skip(15).Join();
                var subbits = bits.Take(length).Join();
                bits = bits.Skip(length).Join();
                while(subbits.Length > 0)
                {
                    packages = ParseRecur(Parse(subbits),packages);

                }
            }
     
            
        }
        return packages;
    }

    public static async Task<object> One()
    {
        var input = (await GetInput())[0].Select(c => c.HexToBits()).Join();
        var packages = ParseRecur(Parse(input), new Package[] {});
        foreach(var p in packages) Console.WriteLine(p.Version);
        var versionSum = packages.Sum(p => p.Version);
        return "Total sum: " + versionSum;
    }

    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
