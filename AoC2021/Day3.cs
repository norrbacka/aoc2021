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

    public static decimal ToDecimal(byte[] bytes)
    {
        //check that it is even possible to convert the array
        if (bytes.Count() != 16)
            throw new Exception("A decimal must be created from exactly 16 bytes");
        //make an array to convert back to int32's
        Int32[] bits = new Int32[4];
        for (int i = 0; i <= 15; i += 4)
        {
            //convert every 4 bytes into an int32
            bits[i / 4] = BitConverter.ToInt32(bytes, i);
        }
        //Use the decimal's new constructor to
        //create an instance of decimal
        return new decimal(bits);
    }
    public static async Task<int> Two() => 2;
        //(await GetInput());
}