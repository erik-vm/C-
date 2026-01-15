namespace BLL;

public static class MoveValidator
{
    public static bool IsValidColumn(Game game, int column)
    {
        return column >= 0 && column < game.Board.Width;
    }

    public static bool IsColumnFull(Game game, int column)
    {
        if (!IsValidColumn(game, column))
            return true;

        return game.Board.IsColumnFull(column);
    }

    public static (bool isValid, string? errorMessage) ValidateMove(Game game, int column)
    {
        if (!IsValidColumn(game, column))
            return (false, $"Column must be between 1 and {game.Board.Width}");

        if (IsColumnFull(game, column))
            return (false, $"Column {column + 1} is full");

        return (true, null);
    }
}