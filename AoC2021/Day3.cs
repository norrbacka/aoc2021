using System.Collections;

public static class Day3
{
    private static async Task<List<bool[]>> GetInput() =>
        await Inputs
        .Read("inputs/day3.txt")
        .Select(text => text.Trim().Select(c => c == '1').ToArray())
        .ToListAsync();

    public static async Task<object> One() 
    {
        var inputs = await GetInput();
        var indexes = Enumerable.Range(0, inputs[0].Count());
        
        var gammaRateData = new List<string>() { };
        foreach (var i in indexes) 
        {
            var d = inputs.Select(input => input[i]);
            var zeros = d.Where(d => !d);
            var ones = d.Where(d => d);
            gammaRateData.Add(zeros.Count() < ones.Count() ? "1" : "0");
        }
        var byteString = string.Join("", gammaRateData);
        var gammaRate = Convert.ToInt32(byteString, 2);
        

        var epsilonData = new List<string>() {  };
        foreach (var j in indexes)
        {
            var d = inputs.Select(input => input[j]);
            var zeros = d.Where(d => !d);
            var ones = d.Where(d => d);
            epsilonData.Add(zeros.Count() > ones.Count() ? "1" : "0");
        }
        byteString = string.Join("", epsilonData);
        var epsilon = Convert.ToInt32(byteString, 2);

        return await Task.Run(() => gammaRate * epsilon);
    }

    public static async Task<object> Two()
    {
        var inputs = await GetInput();

        var ogr = GetOgr(inputs, 0);
        var co2sr = GetC02sr(inputs, 0);
        return await Task.Run(() => ogr*co2sr);
    }

    public static decimal GetOgr(IEnumerable<bool[]> inputs, int index)
    {
        if(inputs.Count() == 1)
        {
            var byteString = string.Join("", inputs.First().Select(i => i ? "1" : "0"));
            var ogr = Convert.ToInt32(byteString, 2);
            return ogr;
        }

        var d = inputs.Select(input => input[index]);
        var zeros = d.Where(d => !d);
        var ones = d.Where(d => d);
        var moreOnes = ones.Count() >= zeros.Count();
        if (moreOnes)
        {
            var toKeep = inputs.Where(input => input[index]);
            return GetOgr(toKeep, index + 1);
        }

        var lessOnes = ones.Count() < zeros.Count();
        if (lessOnes)
        {
            var toKeep = inputs.Where(input => !input[index]);
            return GetOgr(toKeep, index + 1);
        }

        throw new InvalidOperationException();
    }

    public static decimal GetC02sr(IEnumerable<bool[]> inputs, int index)
    {
        if (inputs.Count() == 1)
        {
            var byteString = string.Join("", inputs.First().Select(i => i ? "1" : "0"));
            var ogr = Convert.ToInt32(byteString, 2);
            return ogr;
        }

        var d = inputs.Select(input => input[index]);
        var zeros = d.Where(d => !d);
        var ones = d.Where(d => d);

        var moreOnes = ones.Count() >= zeros.Count();
        if (moreOnes)
        {
            var zerosToKeep = inputs.Where(input => !input[index]);
            return GetC02sr(zerosToKeep, index + 1);
        }

        var lessOnes = ones.Count() < zeros.Count();
        if (lessOnes)
        {
            var onesToKeep = inputs.Where(input => input[index]);
            return GetC02sr(onesToKeep, index + 1);
        }

        throw new InvalidOperationException();
    }

}