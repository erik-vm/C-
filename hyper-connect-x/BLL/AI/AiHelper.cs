namespace BLL.AI;

public static class AiHelper
{
    public static int GetRandomValidMove(Game game)
    {
        var validColumns = new List<int>();
        for (int col = 0; col < game.Board.Width; col++)
        {
            if (!game.Board.IsColumnFull(col))
            {
                validColumns.Add(col);
            }
        }

        if (validColumns.Count == 0)
        {
            throw new InvalidOperationException("No valid moves available");
        }

        return validColumns[Random.Shared.Next(validColumns.Count)];
    }
}
