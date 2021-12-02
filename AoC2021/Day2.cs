using WinstonPuckett.PipeExtensions;

public static class Day2
{
    private static async Task<IList<(string Instruction, int Steps)>> GetInput() =>
        await Inputs
        .Read("inputs/day2.txt")
        .Select(text => (Instruction: text.Split(" ")[0].Trim().ToLower(), Steps: int.Parse(text.Split(" ")[1])))
        .ToListAsync();

    public static async Task<int> One()
    {
        var positions = await GetInput();
        var parsed = positions.Select(p =>
        {
            var x = p.Instruction switch
            {
                "forward" => p.Steps,
                _ => 0
            };
            var y = p.Instruction switch
            {
                "down" => p.Steps,
                "up" => -p.Steps,
                _ => 0
            };
            return (x, y);
        });
        int sumed_x = 0, sumed_y = 0;
        var steps = new List<(int x, int y)>();
        foreach(var p in parsed)
        {
            sumed_x += p.x;
            sumed_y += p.y;
            steps.Add((sumed_x, sumed_y));
        }
        return sumed_x * sumed_y;
    }

    public static async Task<int> Two()
    {
        var positions = await GetInput();
        var parsed = positions.Select(p =>
        {
            var x = p.Instruction switch
            {
                "forward" => p.Steps,
                _ => 0
            };
            var aim = p.Instruction switch
            {
                "down" => p.Steps,
                "up" => -p.Steps,
                _ => 0
            };
            return (x, aim, change: p);
        });
        int sumed_x = 0, sumed_y = 0, sumed_aim = 0;
        var steps = new List<(int x, int y, int aim)>();
        foreach (var p in parsed)
        {

            if (p.change.Instruction == "up")
            {
                sumed_aim += p.aim;
            }
            if (p.change.Instruction == "down")
            {
                sumed_aim += p.aim;
            }
            if (p.change.Instruction == "forward")
            {
                sumed_x += p.change.Steps;
                sumed_y += (p.change.Steps * sumed_aim);
            }
            steps.Add((sumed_x, sumed_y, sumed_aim));
        }
        return sumed_x * sumed_y;
    }
}