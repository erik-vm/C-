namespace DAL.Models;

public class PlayerProfileEntity
{
    public string PlayerName { get; set; } = "";
    public bool IsAi { get; set; }
    public string? AiDifficulty { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalGamesPlayed { get; set; }
    public int TotalWins { get; set; }
}
