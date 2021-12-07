public static class Inputs
{
    public static async IAsyncEnumerable<string> Read(string day)
    {
        using StreamReader reader = File.OpenText($"Inputs/{day}.txt");
        while (reader != null && !reader.EndOfStream)
            yield return (await reader.ReadLineAsync() ?? "") ?? "";
    }
}