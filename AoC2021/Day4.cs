using MoreLinq;

public static class Day4
{
    private static async Task<List<string>> GetInput() =>
        await Inputs
        .Read("inputs/day4.txt")
        .Select(text => text)
        .ToListAsync();

    private static (int[] Numbers, int[][][] Boards) GetGame(List<string> input)
    {
        var numbers = input[0].Split(",").Select(x => int.Parse(x)).ToArray();
        var boards = input
            .Skip(2)
            .Batch(6)
            .Select(board => 
                board
                .Take(5)
                .Select(boardRow => 
                    boardRow
                    .Split(" ")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(x => int.Parse(x))
                    .ToArray())
                .ToArray())
            .ToArray();
        return (numbers, boards);
    }

    public static int[][] Flip(int[][] board) =>
        Enumerable.Range(0, board.First().Count())
        .Select(i => 
            board
            .Select(o => o[i])
            .ToArray())
        .ToArray();

    public static IEnumerable<List<int>> GetDrawnNumbers(int[] numbers)
    {
        var drawn = new List<int>();
        foreach (var number in numbers)
        {
            drawn.Add(number);
            yield return drawn;
        }
    }

    public static int ComputeScore(int[][] board, List<int> drawnNumbers)
    {
        var horizontal = board;        
        var isHorizontalWinner = horizontal.Any(h => h.All(n => drawnNumbers.Contains(n)));
        if (isHorizontalWinner)
        {
            var calledNumber = drawnNumbers.Last();
            var sumOfUnmarked = horizontal.SelectMany(h => h.Where(n => !drawnNumbers.Contains(n))).Sum();
            return sumOfUnmarked * calledNumber;
        }

        var vertical = Flip(board);
        var isVerticalWinner = vertical.Any(v => v.All(n => drawnNumbers.Contains(n)));
        if (isVerticalWinner)
        {
            var calledNumber = drawnNumbers.Last();
            var sumOfUnmarked = vertical.SelectMany(v => v.Where(n => !drawnNumbers.Contains(n))).Sum();
            return sumOfUnmarked * calledNumber;
        }

        return -1;
    }

    public static async Task<object> One()
    {
        var input = await GetInput();
        var (numbers, boards) = GetGame(input);

        foreach(var drawnNumbers in GetDrawnNumbers(numbers))
        {
            foreach(var board in boards)
            {
                var score = ComputeScore(board, drawnNumbers);
                if(score != -1)
                {
                    return score;
                }
            }
        }
        return "No winner";
    }

    public static async Task<object> Two() 
    {
        var input = await GetInput();
        var (numbers, boards) = GetGame(input);
        var winners = new List<(int[][] board, int score)>();

        foreach (var drawnNumbers in GetDrawnNumbers(numbers))
        {
            foreach (var board in boards) //boards.Where(b => winners.Any(w => w.board == b)))
            {
                var score = ComputeScore(board, drawnNumbers);
                if (score != -1 && !winners.Any(w => w.board == board))
                {
                    winners.Add((board, score));
                }
            }
        }

        return winners.Last().score;
    }
}
