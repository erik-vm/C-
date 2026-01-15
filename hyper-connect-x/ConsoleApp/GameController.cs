using BLL;
using DAL;

namespace ConsoleApp;

public static class GameController
{
    private const int AiThinkingDelayMs = 1000;
    private const int AiMoveDisplayDelayMs = 800;
    private const int MenuTransitionDelayMs = 500;

    private static IGameRepository Repository
    {
        get
        {
            var repositoryType = GameSettings.Instance.RepositoryType;
            if (repositoryType.ToLower() == "database")
            {
                var dbContext = new GameDbContext();
                return RepositoryFactory.CreateRepository(repositoryType, dbContext);
            }
            return RepositoryFactory.CreateRepository(repositoryType);
        }
    }


    public static void StartGame(Game? existingGame = null, string? gameId = null)
    {
        var settings = GameSettings.Instance;
        Game game;

        if (existingGame != null)
        {
            game = existingGame;
            Console.WriteLine("\n=== Continuing saved game ===\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        else
        {
            Player player1 = new Player(
                settings.Player1Name,
                settings.Player1Color,
                settings.Player1IsAi,
                settings.Player1AiDifficulty
            );

            Player player2 = new Player(
                settings.Player2Name,
                settings.Player2Color,
                settings.Player2IsAi,
                settings.Player2AiDifficulty
            );

            Player startingPlayer = settings.StartingPlayerName == settings.Player1Name ? player1 : player2;

            game = new Game(
                player1,
                player2,
                settings.BoardHeight,
                settings.BoardWidth,
                settings.BoardShape,
                settings.WinningCondition,
                startingPlayer);

            gameId = "";
        }

        while (!game.IsGameOver)
        {
            Console.Clear();
            GameRenderer.RenderBoard(game);
            Console.WriteLine($"\nCurrent player: {game.CurrentPlayer.Name}");

            int column;

            if (game.CurrentPlayer.IsAi)
            {
                Console.WriteLine($"\n{game.CurrentPlayer.Name} is thinking...");
                Thread.Sleep(AiThinkingDelayMs);
                column = game.CurrentPlayer.GetAiMove(game);
                Console.WriteLine($"{game.CurrentPlayer.Name} chose column {column + 1}");
                Thread.Sleep(AiMoveDisplayDelayMs);
            }
            else
            {
                column = GetHumanMove(game);
                if (column == -1)
                {
                    if (HandleQuit(game, gameId))
                    {
                        return;
                    }

                    continue;
                }
            }

            game.MakeMove(column);
        }


        Console.Clear();
        GameRenderer.RenderBoard(game);

        if (game.IsDraw)
        {
            Console.WriteLine("\n=== It's a draw! ===");
        }
        else
        {
            Console.WriteLine($"\n=== {game.Winner!.Name} wins! ===");
        }


        if (!string.IsNullOrEmpty(gameId))
        {
            Repository.DeleteGame(gameId);
            Console.WriteLine("\n(In-progress save removed - game finished)");
        }

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1. Play Again (same settings)");
        Console.WriteLine("2. Return to Menu");
        Console.Write("\nChoice: ");

        var choice = Console.ReadKey(true).KeyChar;

        if (choice == '1')
        {
            Random random = new Random();
            string startingPlayerName = random.Next(2) == 0 ? settings.Player1Name : settings.Player2Name;
            settings.StartingPlayerName = startingPlayerName;

            StartGame();
        }
        else
        {
            Console.WriteLine("\n\nReturning to menu...");
            Thread.Sleep(MenuTransitionDelayMs);
        }
    }

    private static int GetHumanMove(Game game)
    {
        while (true)
        {
            Console.Write($"{game.CurrentPlayer.Name}'s turn! Enter column (1-{game.Board.Width}) or 'Q' to quit: ");
            string? input = Console.ReadLine();

            if (input?.ToLower() == "q")
            {
                return -1;
            }

            if (!int.TryParse(input, out int column))
            {
                Console.WriteLine("Invalid input! Please enter a number or 'Q' to quit!");
                continue;
            }

            var (isValid, errorMessage) = MoveValidator.ValidateMove(game, column - 1);
            if (!isValid)
            {
                Console.WriteLine(errorMessage);
                continue;
            }

            return column - 1;
        }
    }

    private static bool HandleQuit(Game game, string? gameId)
    {
        Console.Write("\nDo you want to save the game before quitting? (y/n): ");
        string? saveResponse = Console.ReadLine();

        if (saveResponse?.ToLower() == "y")
        {
            GameState gameState = GameStateConverter.ToGameState(game, gameId ?? "");
            gameId = Repository.SaveGame(gameState);
            Console.WriteLine($"\nGame saved! (ID: {gameId})");
        }

        Console.Write("\nAre you sure you want to quit? (y/n): ");
        string? quitResponse = Console.ReadLine();

        if (quitResponse?.ToLower() == "y")
        {
            Console.WriteLine("\nReturning to menu...");
            Thread.Sleep(1000);
            return true;
        }

        return false;
    }
}