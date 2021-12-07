using static System.Console;
using static System.ReadLine;
const string EXIT_MSG = "exit";
static async Task Write(string text) 
{  
    WriteLine(text switch
    {
        "1.1" => await Day1.One(),
        "1.2" => await Day1.Two(),

        "2.1" => await Day2.One(),
        "2.2" => await Day2.Two(),

        "3.1" => await Day3.One(),
        "3.2" => await Day3.Two(),

        "4.1" => await Day4.One(),
        "4.2" => await Day4.Two(),

        "5.1" => await Day5.One(),
        "5.2" => await Day5.Two(),

        "6.1" => await Day6.One(),
        "6.2" => await Day6.Two(),

        "7.1" => await Day7.One(),
        "7.2" => await Day7.Two(),

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