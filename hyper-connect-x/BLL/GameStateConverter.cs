using DAL;

namespace BLL;

public static class GameStateConverter
{
    public static GameState ToGameState(Game game, string gameId = "")
    {
        var gameState = new GameState
        {
            GameId = gameId,
            CreatedAt = DateTime.Now,


            Player1Name = game.Player1.Name,
            Player1Color = game.Player1.Color.ToString(),
            Player1IsAi = game.Player1.IsAi,
            Player1AiDifficulty = game.Player1.AiDifficulty,

            Player2Name = game.Player2.Name,
            Player2Color = game.Player2.Color.ToString(),
            Player2IsAi = game.Player2.IsAi,
            Player2AiDifficulty = game.Player2.AiDifficulty,

            BoardHeight = game.Board.Height,
            BoardWidth = game.Board.Width,
            BoardShape = game.Board.Shape,
            WinningConnection = game.WinningCondition,

            CurrentPlayerName = game.CurrentPlayer.Name,
            IsGameOver = game.IsGameOver,
            WinnerName = game.Winner?.Name,

            BoardCells = ConvertBoardToStringArray(game.Board),

            GameMode = "Custom"
        };

        return gameState;
    }

    public static Game ToGame(GameState gameState)
    {
        if (!Enum.TryParse<ConsoleColor>(gameState.Player1Color, out var player1Color))
        {
            player1Color = GameConfiguration.DefaultPlayer1Color;
        }

        if (!Enum.TryParse<ConsoleColor>(gameState.Player2Color, out var player2Color))
        {
            player2Color = GameConfiguration.DefaultPlayer2Color;
        }

        Player player1 = new Player(
            gameState.Player1Name,
            player1Color,
            gameState.Player1IsAi,
            gameState.Player1AiDifficulty
        );

        Player player2 = new Player(
            gameState.Player2Name,
            player2Color,
            gameState.Player2IsAi,
            gameState.Player2AiDifficulty
        );

        Player startingPlayer = gameState.CurrentPlayerName == player1.Name ? player1 : player2;

        Game game = new Game(
            player1,
            player2,
            gameState.BoardHeight,
            gameState.BoardWidth,
            gameState.BoardShape,
            gameState.WinningConnection,
            startingPlayer
        );

        RestoreBoardState(game, gameState.BoardCells, player1, player2);

        // Restore game over state
        game.IsGameOver = gameState.IsGameOver;
        if (!string.IsNullOrEmpty(gameState.WinnerName))
        {
            game.Winner = gameState.WinnerName == player1.Name ? player1 : player2;
        }

        return game;
    }

    private static string[][] ConvertBoardToStringArray(Board board)
    {
        string[][] cells = new string[board.Height][];

        for (int row = 0; row < board.Height; row++)
        {
            cells[row] = new string[board.Width];
            for (int col = 0; col < board.Width; col++)
            {
                Player? player = board.GetCell(row, col);
                cells[row][col] = player?.Name ?? "";
            }
        }

        return cells;
    }


    private static void RestoreBoardState(Game game, string[][] boardCells, Player player1, Player player2)
    {
        if (boardCells == null || boardCells.Length == 0)
        {
            return;
        }

        for (int row = 0; row < game.Board.Height && row < boardCells.Length; row++)
        {
            if (boardCells[row] == null)
            {
                continue;
            }

            for (int col = 0; col < game.Board.Width && col < boardCells[row].Length; col++)
            {
                string cellValue = boardCells[row][col];
                if (!string.IsNullOrEmpty(cellValue))
                {
                    Player player = cellValue == player1.Name ? player1 : player2;
                    game.Board.SetCell(row, col, player);
                }
            }
        }
    }
}