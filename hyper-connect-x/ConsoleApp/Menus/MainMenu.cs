using BLL;
using DAL;

namespace ConsoleApp.Menus;

public class MainMenu : BaseMenu
{
    private readonly MenuManager _menuManager;

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

    public MainMenu(MenuManager menuManager) : base("CONNECT 4 - MAIN MENU", EMenuType.Main)
    {
        _menuManager = menuManager;
        InitializeOptions();
    }

    protected sealed override void InitializeOptions()
    {
        Options.Add(new MenuOption('1', "Start New Game", StartGame));
        Options.Add(new MenuOption('2', "Continue Game", ContinueGame));
        Options.Add(new MenuOption('3', "Delete Saved Game", DeleteGame));
        Options.Add(new MenuOption('4', "Settings", OpenSettings));
        Options.Add(new MenuOption('X', "Exit", Exit));
    }

    private void StartGame()
    {
        _menuManager.NavigateTo(EMenuType.Start);
    }

    private void ContinueGame()
    {
        Console.Clear();
        Console.WriteLine("=== CONTINUE GAME ===\n");

        var games = Repository.GetAllGames();

        if (games.Count == 0)
        {
            Console.WriteLine("No saved games found!");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Select a game to continue:\n");

        for (int i = 0; i < games.Count; i++)
        {
            var game = games[i];
            string status = game.IsGameOver ? "Finished" : "In Progress";
            string dateStr = (game.UpdatedAt ?? game.CreatedAt).ToString("yyyy-MM-dd HH:mm");

            Console.WriteLine($"{i + 1}. {game.Player1Name} vs {game.Player2Name}");
            Console.WriteLine($"   Status: {status} | {game.BoardHeight}x{game.BoardWidth} {game.BoardShape}");
            Console.WriteLine($"   Saved: {dateStr}");
            Console.WriteLine();
        }

        Console.WriteLine("0. Back to Main Menu");
        Console.Write("\nEnter choice: ");

        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice < 0 || choice > games.Count)
        {
            Console.WriteLine("Invalid choice!");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            return;
        }

        if (choice == 0)
        {
            return;
        }

        var selectedGame = games[choice - 1];
        Game loadedGame = GameStateConverter.ToGame(selectedGame);
        GameController.StartGame(loadedGame, selectedGame.GameId);
    }

    private void DeleteGame()
    {
        Console.Clear();
        Console.WriteLine("=== DELETE SAVED GAME ===\n");

        var games = Repository.GetAllGames();

        if (games.Count == 0)
        {
            Console.WriteLine("No saved games found!");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Select a game to delete:\n");

        for (int i = 0; i < games.Count; i++)
        {
            var game = games[i];
            string status = game.IsGameOver ? "Finished" : "In Progress";
            string dateStr = (game.UpdatedAt ?? game.CreatedAt).ToString("yyyy-MM-dd HH:mm");

            Console.WriteLine($"{i + 1}. {game.Player1Name} vs {game.Player2Name}");
            Console.WriteLine($"   Status: {status} | {game.BoardHeight}x{game.BoardWidth} {game.BoardShape}");
            Console.WriteLine($"   Saved: {dateStr}");
            Console.WriteLine();
        }

        Console.WriteLine("0. Back to Main Menu");
        Console.Write("\nEnter choice: ");

        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice < 0 || choice > games.Count)
        {
            Console.WriteLine("Invalid choice!");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            return;
        }

        if (choice == 0)
        {
            return;
        }

        var selectedGame = games[choice - 1];
        Console.Write($"\nAre you sure you want to delete this game? (y/n): ");
        var confirm = Console.ReadLine();

        if (confirm?.ToLower() == "y")
        {
            var deleted = Repository.DeleteGame(selectedGame.GameId);
            Console.WriteLine(deleted ? "\nGame deleted successfully!" : "\nFailed to delete game!");
        }
        else
        {
            Console.WriteLine("\nDeletion cancelled.");
        }

        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    private void OpenSettings()
    {
        _menuManager.NavigateTo(EMenuType.Settings);
    }
}