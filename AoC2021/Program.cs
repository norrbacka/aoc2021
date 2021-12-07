using System.Reflection;
using System.Text.RegularExpressions;
using static System.Console;
using static System.ReadLine;
const string EXIT_MSG = "exit";
static async Task<object> GetDay(string text)
{
    try
    {
        var regexMatch = Regex.Match(text, @"(?<dayNumber>\d*)(\.)(?<task>\d*)");
        if (!regexMatch.Success) return "Cannot find what you are looking for!";
        var dayNumber = regexMatch.Groups["dayNumber"].Value;
        if(dayNumber.Length == 1) dayNumber = "0" + dayNumber;
        var task = regexMatch.Groups["task"].Value;
        var type = Type.GetType($"Day{dayNumber}");
        var funcName = task switch
        {
            "1" => "One",
            "2" => "Two",
            _ => null
        };
        if (funcName == null) return "No method with that name";
        var method = type?.GetMethod(funcName, BindingFlags.Public | BindingFlags.Static);
        var funcCall = method?.Invoke(null, null);
        if (funcCall == null) return "Task is not callable";
        var taskFuncCall = ((Task)funcCall);
        await taskFuncCall.ConfigureAwait(false);
        var resultProperty = taskFuncCall.GetType().GetProperty("Result");
        return resultProperty?.GetValue(taskFuncCall) ?? "No result fom Task";
    } catch
    {
        return "Nothing ado about anything!";
    }
}
static async Task Write(string text) 
{  
    WriteLine(text switch
    {
        EXIT_MSG => "exiting...",
        "-h" => "Day number and task number separated with dot, like 1.1, 1.2, 23.1. 'exit' to quit.",
        _ => await GetDay(text)
    });
    if(text != EXIT_MSG)
    {
        await Write(Read("Day>"));
    }
}
await Write(Read("Day (-h for help)>"));