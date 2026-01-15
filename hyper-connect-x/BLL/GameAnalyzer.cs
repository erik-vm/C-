using DAL;

namespace BLL;

public static class GameAnalyzer
{
    public static int CountMoves(Game game)
    {
        int count = 0;
        for (int row = 0; row < game.Board.Height; row++)
        {
            for (int col = 0; col < game.Board.Width; col++)
            {
                if (game.Board.GetCell(row, col) != null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public static int CountMovesFromState(GameState gameState)
    {
        int count = 0;
        if (gameState.BoardCells != null)
        {
            for (int i = 0; i < gameState.BoardCells.Length; i++)
            {
                for (int j = 0; j < gameState.BoardCells[i].Length; j++)
                {
                    if (!string.IsNullOrEmpty(gameState.BoardCells[i][j]))
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    public static bool IsGameCompleted(GameState gameState)
    {
        int pieceCount = CountMovesFromState(gameState);
        int totalCells = gameState.BoardHeight * gameState.BoardWidth;
        return gameState.IsGameOver || pieceCount >= gameState.WinningConnection || pieceCount == totalCells;
    }
}
