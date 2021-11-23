using static System.Console;
using static System.ReadLine;
const string EXIT_MSG = "exit";
static async Task Write(string text) 
{  
    WriteLine(text switch
    {
        "test" => await Test.Run(),
        EXIT_MSG => "exiting...",
        "-h" => "Day number and task letter, like 1a, 2b, 15a. 'exit' to quit.",
        _ => "Idk? Try again."
    });
    if(text != EXIT_MSG)
    {
        await Write(Read("Day>"));
    }
}
await Write(Read("Day (-h for help)>"));