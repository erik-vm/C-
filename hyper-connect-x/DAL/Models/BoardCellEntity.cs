namespace DAL.Models;

public class BoardCellEntity
{
    public int Id { get; init; }

    public string GameStateId { get; init; } = "";

    public int Row { get; init; }
    public int Column { get; init; }
    public string PlayerName { get; init; } = "";

    public GameStateEntity? GameState { get; init; }
}