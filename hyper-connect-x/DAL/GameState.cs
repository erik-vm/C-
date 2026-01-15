namespace DAL;

public class GameState
{
    public string GameId { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string Player1Name { get; init; } = "";
    public string Player1Color { get; init; } = "";
    public bool Player1IsAi { get; init; }
    public string? Player1AiDifficulty { get; init; }

    public string Player2Name { get; init; } = "";
    public string Player2Color { get; init; } = "";
    public bool Player2IsAi { get; init; }
    public string? Player2AiDifficulty { get; init; }

    public int BoardHeight { get; init; }
    public int BoardWidth { get; init; }
    public string BoardShape { get; init; } = "";
    public int WinningConnection { get; init; }

    public string CurrentPlayerName { get; init; } = "";
    public bool IsGameOver { get; init; }
    public string? WinnerName { get; init; }

    public string[][] BoardCells { get; init; } = [];

    public string GameMode { get; init; } = "";
}