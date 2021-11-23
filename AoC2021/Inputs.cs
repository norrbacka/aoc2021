public static class Inputs
{
    public static async IAsyncEnumerable<string> Read(string fileName)
    {
        using StreamReader reader = File.OpenText(fileName);
        while (reader != null && !reader.EndOfStream)
            yield return await reader.ReadLineAsync();
    }
}