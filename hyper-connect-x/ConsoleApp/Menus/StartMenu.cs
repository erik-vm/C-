using DAL;

namespace ConsoleApp.Menus;

public class StartMenu : BaseMenu
{
    private readonly MenuManager? _menuManager;
    private readonly Random _random = new Random();

    public StartMenu() : base("START NEW GAME - SELECT MODE", EMenuType.Start)
    {
        InitializeOptions();
    }

    public StartMenu(MenuManager menuManager) : base("START NEW GAME - SELECT MODE", EMenuType.Start)
    {
        _menuManager = menuManager;
        InitializeOptions();
    }

    protected sealed override void InitializeOptions()
    {
        Options.Add(new MenuOption('1', "Player vs Player", PlayerVsPlayer));
        Options.Add(new MenuOption('2', "Player vs Computer", PlayerVsComputer));
        Options.Add(new MenuOption('3', "Computer vs Computer", ComputerVsComputer));
        Options.Add(new MenuOption('B', "Back to previous menu", Back));
    }

    private void PlayerVsPlayer()
    {
        Console.Clear();
        Console.WriteLine("=== PLAYER VS PLAYER ===");
        Console.WriteLine();

        var settings = GameSettings.Instance;

        Console.Write($"Enter Player 1 name (current: {settings.Player1Name}): ");
        var player1Name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(player1Name))
            settings.Player1Name = player1Name;

        Console.Write($"Enter Player 2 name (current: {settings.Player2Name}): ");
        var player2Name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(player2Name))
            settings.Player2Name = player2Name;

        settings.Player1IsAi = false;
        settings.Player2IsAi = false;

        SelectGameType();
    }

    private void PlayerVsComputer()
    {
        Console.Clear();
        Console.WriteLine("=== PLAYER VS COMPUTER ===");
        Console.WriteLine();

        var settings = GameSettings.Instance;

        Console.Write($"Enter your name (current: {settings.Player1Name}): ");
        var playerName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(playerName))
            settings.Player1Name = playerName;

        var aiName = GetRandomAiName();
        settings.Player2Name = aiName;

        Console.WriteLine();
        Console.WriteLine($"Select AI difficulty for {aiName}:");
        Console.WriteLine("1. Easy");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. Hard");
        Console.Write("Choice: ");

        var difficultyChoice = Console.ReadKey(true).KeyChar;
        var difficulty = difficultyChoice switch
        {
            '1' => "Easy",
            '2' => "Medium",
            '3' => "Hard",
            _ => "Medium"
        };

        settings.Player2AiDifficulty = difficulty;
        settings.Player1IsAi = false;
        settings.Player2IsAi = true;

        Console.WriteLine($"\n\nYou will play against {aiName} (Difficulty: {difficulty})");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        SelectGameType();
    }

    private void ComputerVsComputer()
    {
        Console.Clear();
        Console.WriteLine("=== COMPUTER VS COMPUTER ===");
        Console.WriteLine();

        var aiNames = AiProfileService.GetAllAiNames();
        var ai1Name = SelectAiFromList(aiNames, "Select AI Player 1:");
        if (ai1Name == null) return;

        var ai2Name = SelectAiFromList(aiNames.Where(n => n != ai1Name).ToList(), "Select AI Player 2:");
        if (ai2Name == null) return;

        var settings = GameSettings.Instance;
        settings.Player1Name = ai1Name;
        settings.Player2Name = ai2Name;

        Console.Clear();
        Console.WriteLine($"AI 1: {ai1Name}");
        Console.WriteLine("Select difficulty:");
        Console.WriteLine("1. Easy");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. Hard");
        Console.Write("Choice: ");

        var difficulty1Choice = Console.ReadKey(true).KeyChar;
        var difficulty1 = difficulty1Choice switch
        {
            '1' => "Easy",
            '2' => "Medium",
            '3' => "Hard",
            _ => AiProfileService.GetDefaultDifficulty(ai1Name)
        };

        Console.WriteLine($"\n\nAI 2: {ai2Name}");
        Console.WriteLine("Select difficulty:");
        Console.WriteLine("1. Easy");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. Hard");
        Console.Write("Choice: ");

        var difficulty2Choice = Console.ReadKey(true).KeyChar;
        var difficulty2 = difficulty2Choice switch
        {
            '1' => "Easy",
            '2' => "Medium",
            '3' => "Hard",
            _ => AiProfileService.GetDefaultDifficulty(ai2Name)
        };

        settings.Player1IsAi = true;
        settings.Player2IsAi = true;
        settings.Player1AiDifficulty = difficulty1;
        settings.Player2AiDifficulty = difficulty2;

        Console.WriteLine($"\n\n{ai1Name} ({difficulty1}) vs {ai2Name} ({difficulty2})");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        SelectGameType();
    }

    private void SelectGameType()
    {
        Console.Clear();
        Console.WriteLine("=== SELECT GAME TYPE ===");
        Console.WriteLine();
        Console.WriteLine("1. Classical (6x7 board, win: 4 in a row)");
        Console.WriteLine("2. Custom");
        Console.Write("\nChoice: ");

        var choice = Console.ReadKey(true).KeyChar;

        if (choice == '1')
        {
            var settings = GameSettings.Instance;
            settings.GameMode = "Classical";
            settings.BoardHeight = 6;
            settings.BoardWidth = 7;
            settings.WinningCondition = 4;

            Console.WriteLine("\n\nClassical game selected!");
            Console.WriteLine("Press any key to start game...");
            Console.ReadKey();

            StartGame();
        }
        else if (choice == '2')
        {
            var settings = GameSettings.Instance;
            settings.GameMode = "Custom";

            if (_menuManager != null)
            {
                _menuManager.NavigateTo(EMenuType.CustomGame);
            }
            else
            {
                Console.WriteLine("\n\nCustom game configuration...");
                Console.WriteLine("(Custom menu not yet wired)");
                Console.ReadKey();
            }
        }
        else
        {
            Console.WriteLine("\n\nInvalid choice. Returning to menu...");
            Console.ReadKey();
        }
    }

    private void StartGame()
    {
        Console.Clear();
        Console.WriteLine("=== GAME STARTING ===\n");

        var settings = GameSettings.Instance;

        Random random = new Random();
        string startingPlayerName = random.Next(2) == 0 ? settings.Player1Name : settings.Player2Name;
        settings.StartingPlayerName = startingPlayerName;

        Console.WriteLine($"Player 1: {settings.Player1Name}");
        Console.WriteLine($"Player 2: {settings.Player2Name}");
        Console.WriteLine($"Board Shape: {settings.BoardShape}");
        Console.WriteLine($"Board Size: {settings.BoardHeight}h x {settings.BoardWidth}w");
        Console.WriteLine($"Win Condition: {settings.WinningCondition} in a row");
        Console.WriteLine($"Game Mode: {settings.GameMode}");
        Console.WriteLine($"\n{startingPlayerName} will start first!");

        Console.WriteLine("\nPress any key to start the game...");
        Console.ReadKey();

        GameController.StartGame();
    }
    
    private string GetRandomAiName()
    {
        var aiNames = AiProfileService.GetAllAiNames();
        return aiNames[_random.Next(aiNames.Count)];
    }

    private string? SelectAiFromList(List<string> aiNames, string prompt)
    {
        Console.Clear();
        Console.WriteLine(prompt);
        Console.WriteLine();

        for (int i = 0; i < aiNames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {aiNames[i]}");
        }

        Console.WriteLine("0. Cancel");
        Console.Write("\nChoice: ");

        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice < 0 || choice > aiNames.Count)
        {
            Console.WriteLine("Invalid choice!");
            Console.ReadKey();
            return null;
        }

        if (choice == 0)
        {
            return null;
        }

        return aiNames[choice - 1];
    }
}