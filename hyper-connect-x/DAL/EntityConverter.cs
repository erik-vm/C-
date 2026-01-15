using DAL.Models;

namespace DAL;

public static class EntityConverter
{
    public static GameStateEntity ToEntity(GameState gameState)
    {
        var entity = new GameStateEntity
        {
            GameId = gameState.GameId,
            CreatedAt = gameState.CreatedAt,
            UpdatedAt = gameState.UpdatedAt,
            Player1Name = gameState.Player1Name,
            Player1Color = gameState.Player1Color,
            Player1IsAi = gameState.Player1IsAi,
            Player1AiDifficulty = gameState.Player1AiDifficulty,
            Player2Name = gameState.Player2Name,
            Player2Color = gameState.Player2Color,
            Player2IsAi = gameState.Player2IsAi,
            Player2AiDifficulty = gameState.Player2AiDifficulty,
            BoardHeight = gameState.BoardHeight,
            BoardWidth = gameState.BoardWidth,
            BoardShape = gameState.BoardShape,
            WinningConnection = gameState.WinningConnection,
            CurrentPlayerName = gameState.CurrentPlayerName,
            IsGameOver = gameState.IsGameOver,
            WinnerName = gameState.WinnerName,
            GameMode = gameState.GameMode
        };

        for (int row = 0; row < gameState.BoardHeight; row++)
        {
            for (int col = 0; col < gameState.BoardWidth; col++)
            {
                string cellValue = gameState.BoardCells[row][col];
                if (!string.IsNullOrEmpty(cellValue))
                {
                    entity.BoardCells.Add(new BoardCellEntity
                    {
                        GameStateId = gameState.GameId,
                        Row = row,
                        Column = col,
                        PlayerName = cellValue
                    });
                }
            }
        }

        return entity;
    }

    public static GameState ToGameState(GameStateEntity entity)
    {
        var gameState = new GameState
        {
            GameId = entity.GameId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Player1Name = entity.Player1Name,
            Player1Color = entity.Player1Color,
            Player1IsAi = entity.Player1IsAi,
            Player1AiDifficulty = entity.Player1AiDifficulty,
            Player2Name = entity.Player2Name,
            Player2Color = entity.Player2Color,
            Player2IsAi = entity.Player2IsAi,
            Player2AiDifficulty = entity.Player2AiDifficulty,
            BoardHeight = entity.BoardHeight,
            BoardWidth = entity.BoardWidth,
            BoardShape = entity.BoardShape,
            WinningConnection = entity.WinningConnection,
            CurrentPlayerName = entity.CurrentPlayerName,
            IsGameOver = entity.IsGameOver,
            WinnerName = entity.WinnerName,
            GameMode = entity.GameMode,
            BoardCells = new string[entity.BoardHeight][]
        };

        for (int i = 0; i < entity.BoardHeight; i++)
        {
            gameState.BoardCells[i] = new string[entity.BoardWidth];
            for (int j = 0; j < entity.BoardWidth; j++)
            {
                gameState.BoardCells[i][j] = "";
            }
        }

        foreach (var cell in entity.BoardCells)
        {
            gameState.BoardCells[cell.Row][cell.Column] = cell.PlayerName;
        }

        return gameState;
    }
}