namespace DAL.Models;

public class GameStateEntity
{
    public string GameId { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string Player1Name { get; set; } = "";
    public string Player1Color { get; set; } = "";
    public bool Player1IsAi { get; set; }
    public string? Player1AiDifficulty { get; set; }

    public string Player2Name { get; set; } = "";
    public string Player2Color { get; set; } = "";
    public bool Player2IsAi { get; set; }
    public string? Player2AiDifficulty { get; set; }

    public int BoardHeight { get; set; }
    public int BoardWidth { get; set; }
    public string BoardShape { get; set; } = "";
    public int WinningConnection { get; set; }

    public string CurrentPlayerName { get; set; } = "";
    public bool IsGameOver { get; set; }
    public string? WinnerName { get; set; }
    public string GameMode { get; set; } = "";

    public List<BoardCellEntity> BoardCells { get; set; } = new();
}