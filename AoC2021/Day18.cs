using System.Reflection;
using System.Text.RegularExpressions;
using LanguageExt;

public static class Day18
{
    private static async Task<List<string>> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToListAsync();

    const string IsPair = @"\[\d,\d\]";
    const string AnyDoubleDigits = @"\d\d";

    record SfNr(string Value)
    {

        public (int s, int e) ExplosionIndex(string v)
        {
            var matches = Regex.Matches(v, IsPair);
            foreach (Match match in matches)
            {
                var isNestedFromLeft = v[..match.Index].Count(c => c == '[') == 4;
                if (isNestedFromLeft)
                {
                    return (match.Index, match.Index + 5);
                }
            }
            throw new Exception("Should only be called if it can explode!");
        }

        public bool ShouldExplode(string v) {
            var matches = Regex.Matches(v, IsPair);
            if(matches.Count == 0) return false;
            foreach(Match match in matches) {   
                var isNestedFromLeft = v[..match.Index].Count(c => c == '[') == 4;
                if(isNestedFromLeft) return true;
            }
            return false;
        }

        public int SplitIndex(string v) => Regex.Matches(v, AnyDoubleDigits).First().Index;

        public bool ShouldSplit(string v) => 
            Regex.Matches(v, AnyDoubleDigits).Count > 0;

        public SfNr Reduce()
        {
            var value = Value;
            while(ShouldExplode(value) || ShouldSplit(value))
            {
                if (ShouldExplode(value))
                {
                    var (s, e) = ExplosionIndex(value);
                    if(value[s-1] != '[')
                    {
                        var leftNumber = int.Parse(value[s - 2].ToString());
                        var additionNumber = int.Parse(value[s + 1].ToString());
                        var newNumber = leftNumber + additionNumber;
                        value = value[..(s-3)] + $"[{newNumber}, 0]" + value[(e+1)..];

                    } else
                    {
                        var rightNumber = int.Parse(value[e + 1].ToString());
                        var additionNumber = int.Parse(value[e - 2].ToString());
                        var newNumber = rightNumber + additionNumber;
                        value = value[..(s-1)] + $"[0, {newNumber}]" + value[(e+3)..];
                    }
                }
                else
                {
                    var splitIndex = SplitIndex(value);
                    var number = int.Parse(value[splitIndex].ToString() + value[splitIndex+1].ToString());
                    var even = number % 2 == 0;
                    var firstNumber = number / 2;
                    var secondNumber = firstNumber + (even ? 0 : 1);
                    value = value[..(splitIndex)] + $"[{firstNumber},{secondNumber}]" + value[(splitIndex+2)..];
                }
            }
            return new(value);
        }
    }

    public static async Task<object> One()
    {
        Console.WriteLine(new SfNr("[[[[[9,8],1],2],3],4]").Reduce().Value);
        Console.WriteLine(new SfNr("[7,[6,[5,[4,[3,2]]]]]").Reduce().Value);
        Console.WriteLine(new SfNr("[[[[0,7],4],[15,[0,13]]],[1,1]]").Reduce().Value);
        return 1337;
    }
    public static async Task<object> Two() => await Task.Run(() => "Not yet implemented");
}
