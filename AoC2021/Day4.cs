using LanguageExt;
using MoreLinq;
using WinstonPuckett.PipeExtensions;

public static class Day4
{
    static async Task<List<string>> GetInput() =>
        await Inputs
        .Read("inputs/day4.txt")
        .Select(text => text)
        .ToListAsync();

    static (int[] Numbers, int[][][] Boards) GetGame(List<string> input)
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

    static int[][] Flip(int[][] board) =>
        Enumerable.Range(0, board.First().Count())
        .Select(i =>
            board
            .Select(o => o[i])
            .ToArray())
        .ToArray();

    static IEnumerable<List<int>> GetDrawnNumbers(int[] numbers)
    {
        var drawn = new List<int>();
        foreach (var number in numbers)
        {
            drawn.Add(number);
            yield return drawn;
        }
    }

    static bool IsWinner(int[][] board, List<int> drawnNumbers) =>
        board.Any(row => row.All(n => drawnNumbers.Contains(n)));

    private static int GetUnmarkedNumbersSum(int[][] board, List<int> drawnNumbers) =>
        board.SelectMany(b => b.Where(n => !drawnNumbers.Contains(n))).Sum();

    static Option<int> MaybeComputeScore(int[][] board, List<int> drawnNumbers) =>
        IsWinner(board, drawnNumbers) ? drawnNumbers.Last() * GetUnmarkedNumbersSum(board, drawnNumbers) : Option<int>.None;

    static Option<int> MaybeGetScore(int[][] board, List<int> drawnNumbers) =>
        MaybeComputeScore(board, drawnNumbers).Match(s => s, () =>
        MaybeComputeScore(Flip(board), drawnNumbers));

    static IEnumerable<int[][]> GetRemainingBoards(IEnumerable<int[][]> boards, IEnumerable<(int[][] board, int score)> winners) =>
        boards.Where(b => !winners.Any(w => w.board == b));

    static List<(int[][] board, int score)> GetWinners(int[] numbers, int[][][] boards)
    {
        var winners = new List<(int[][] board, int score)>();
        foreach (var drawnNumbers in GetDrawnNumbers(numbers))
        {
            foreach (var board in GetRemainingBoards(boards, winners))
            {
                var score = MaybeGetScore(board, drawnNumbers);
                score.IfSome(s => { winners.Add((board, s)); });
            }
        }
        return winners;
    }

    private static async Task<List<(int[][] board, int score)>> GetWinners() => 
        (await GetInput())
        .Pipe(GetGame)
        .Pipe((n, b) => GetWinners(n, b));

    public static async Task<object> One() => (await GetWinners()).First().score;

    public static async Task<object> Two() => (await GetWinners()).Last().score;
}