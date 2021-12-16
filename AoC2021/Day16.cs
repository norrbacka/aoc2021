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

    static string Header(this string text) => text.Take(6).Join();
    static int PacketVersion(this string text) => text.Take(3).ToInt();
    static int TypeId(this string text) => text.Skip(3).Take(3).ToInt();
    static int LengthTypeId(this string text) => text.Skip(6).Take(1).ToInt();


    static decimal LiteralValue(this string[] text)
    {
        var data = text.Join().Skip(6).Join();
        var groups = data
            .Batch(5)
            .Where(x => x.Count() == 5)
            .TakeUntil(x => x.First() == '0')
            .Select(x => x.Skip(1).Join());
        var toDecimal = groups.ToInt();
        return toDecimal;
    }


    static List<string> GetPackages(this string line, List<string> packages)
    {
        var header = line.Header();
        var packetVersion = header.PacketVersion();
        var typeId = header.TypeId();

        if(typeId == 4)
        {
            //packages.Add(line.Join());
            //var body = line.LiteralValue();
            return packages;
        }

        var lengthTypeId = line.Join().LengthTypeId();
        if(lengthTypeId == 0)
        {
            var lengthOfSubpackages = line.Skip(7).Take(15).ToInt();
            var numberOf16Bits = lengthOfSubpackages / 16;
            var startingPackage = lengthOfSubpackages - (numberOf16Bits*16);
            var subPackageText = line.Skip(7).Skip(15).Take(lengthOfSubpackages).Join();
            var takesAndSkips = new List<(int skip, int take)>();
            if(startingPackage > 0)
            {
                takesAndSkips.Add((7+15, startingPackage));
            }
            foreach(var i in Enumerable.Range(0, numberOf16Bits))
            {
                var last = takesAndSkips.Any() ? takesAndSkips.Last() : (skip: 7+15, take: 0);
                takesAndSkips.Add((last.skip + last.take, 16));
            }
            foreach (var (skip, take) in takesAndSkips)
            {
                var package = line.Skip(skip).Take(take).Join();
                packages.Add(package);
            }
            return GetPackages(subPackageText, packages);
        }
        else
        {
            var numberOfSubPackages = line.Skip(7).Take(11).ToInt();
            for (int i = 0; i < numberOfSubPackages; i++)
            {
                var subpackage = line.Skip(7).Skip(11).Skip((i)*11).Take(11).Join();
                packages.Add(subpackage);
                //packages = GetPackages(subpackage, packages);
            }
            return packages;
        }
    }

    static int Version(this string package)
    {
        var header = package.Header();
        var packetVersion = header.PacketVersion();
        return packetVersion;
    }

    public static async Task<object> One()
    {
        var input = await GetInput();
        var lines = input.Select(line => line.Select(c => c.HexToBits()).ToArray()).ToArray();
        var sums = new List<int>();
        foreach (var line in lines)
        {
            var packages = new List<string> { line.Join() };
            packages = line.Join().GetPackages(packages);
            var sumOfVersions = packages.Sum(x => x.Version());
            Console.WriteLine($"{line.Join()} => {sumOfVersions}");
            sums.Add(sumOfVersions);
        }
        return "Total sum: " + sums.Sum();
    }
    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
