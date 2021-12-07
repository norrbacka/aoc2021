using LanguageExt;
using MoreLinq;
using System.Reflection;
using WinstonPuckett.PipeExtensions;

public static class Day4
{
    static async Task<List<string>> GetInput() =>
        await Inputs
        .Read(MethodBase.GetCurrentMethod()?.DeclaringType?.FullName?.Split("+").First() ?? "")
        .Select(text => text)
        .ToListAsync();

    static (int[] Numbers, int[][][] Boards) GetGame(List<string> input) => (
        input[0].Split(",").Select(x => int.Parse(x)).ToArray(),
        input
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
        .ToArray()).ToArray()).ToArray());

    static int[][] Flip(int[][] board) =>
        Enumerable.Range(0, board.First().Count())
        .Select(i => board.Select(o => o[i]).ToArray()).ToArray();

    static IEnumerable<IEnumerable<int>> GetDrawnNumbersRecur(IEnumerable<int> numbers, IEnumerable<IEnumerable<int>> sequence) =>
        numbers.Any() ?
            numbers
            .Pipe(n => (remaining: n.Skip(1), toAdd: numbers.Take(1)))
            .Pipe((remaining, toAdd) => 
            (
                remaining,
                MoreEnumerable.Append(sequence, (sequence.LastOrDefault() ?? Array.Empty<int>()).Append(toAdd))
            ))
            .Pipe((remaining, sequence) => GetDrawnNumbersRecur(remaining, sequence)) 
        :
            sequence;

    static bool IsWinner(int[][] board, IEnumerable<int> drawnNumbers) =>
        board.Any(row => row.All(n => drawnNumbers.Contains(n)));

    static int GetUnmarkedNumbersSum(int[][] board, IEnumerable<int> drawnNumbers) =>
        board.SelectMany(b => b.Where(n => !drawnNumbers.Contains(n))).Sum();

    static Option<int> MaybeComputeScore(int[][] board, IEnumerable<int> drawnNumbers) =>
        IsWinner(board, drawnNumbers) ? drawnNumbers.Last() * GetUnmarkedNumbersSum(board, drawnNumbers) : Option<int>.None;

    static Option<int> MaybeGetScore(int[][] board, IEnumerable<int> drawnNumbers) =>
        MaybeComputeScore(board, drawnNumbers)
        .Match(
            scoreForBoardIfAnyRowIsWinner => scoreForBoardIfAnyRowIsWinner, 
            () => MaybeComputeScore(Flip(board), drawnNumbers)
            .Match(
                scoreForBoardIfAnyColumnIsWinner => scoreForBoardIfAnyColumnIsWinner, 
                () => Option<int>.None)); //NOT A WINNER!

    static IEnumerable<int[][]> GetRemainingBoards(IEnumerable<int[][]> boards, IEnumerable<(int[][] board, int score)> winners) =>
        boards.Where(b => !winners.Any(w => w.board == b));

    static List<(int[][] board, int score)> GetWinners(int[] numbers, int[][][] boards)
    {
        var winners = new List<(int[][] board, int score)>();
        foreach (var drawnNumbers in GetDrawnNumbersRecur(numbers, Array.Empty<int[]>()))
        {
            foreach (var board in GetRemainingBoards(boards, winners))
            {
                var score = MaybeGetScore(board, drawnNumbers);
                score.IfSome(s => { winners.Add((board, s)); });
            }
        }
        return winners;
    }

    static async Task<List<(int[][] board, int score)>> GetWinners() => 
        (await GetInput())
        .Pipe(GetGame)
        .Pipe((n, b) => GetWinners(n, b));

    public static async Task<object> One() => (await GetWinners()).First().score;

    public static async Task<object> Two() => (await GetWinners()).Last().score;
}