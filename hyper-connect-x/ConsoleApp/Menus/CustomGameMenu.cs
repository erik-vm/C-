namespace ConsoleApp.Menus;

public class CustomGameMenu : BaseMenu
{
    private const int MinBoardDimension = 3;
    private const int MaxBoardDimension = 20;
    private const int MinWinCondition = 2;

    private readonly MenuManager? _menuManager;
    private readonly GameSettings _settings;

    public CustomGameMenu() : base("CUSTOM GAME SETUP", EMenuType.CustomGame)
    {
        _settings = GameSettings.Instance;
        InitializeOptions();
    }

    public CustomGameMenu(MenuManager menuManager) : base("CUSTOM GAME SETUP", EMenuType.CustomGame)
    {
        _menuManager = menuManager;
        _settings = GameSettings.Instance;
        InitializeOptions();
    }

    protected override void Display()
    {
        Console.Clear();
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine(Title);
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();

        Console.WriteLine("Current Configuration:");
        Console.WriteLine($"  Shape: {_settings.BoardShape}");
        Console.WriteLine($"  Board Size: {_settings.BoardHeight}h x {_settings.BoardWidth}w");
        Console.WriteLine($"  Winning Connection: {_settings.WinningCondition}");
        Console.WriteLine();
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();

        for (int i = 0; i < Options.Count; i++)
        {
            if (i == SelectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"> [{Options[i].Shortcut}] {Options[i].DisplayText}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  [{Options[i].Shortcut}] {Options[i].DisplayText}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Use ↑↓ arrows to navigate, Enter to select, or press shortcut key");
    }

    protected sealed override void InitializeOptions()
    {
        Options.Add(new MenuOption('1', "Select Board Shape", SelectBoardShape));
        Options.Add(new MenuOption('2', "Set Board Size (Height x Width)", SetBoardSize));
        Options.Add(new MenuOption('3', "Set Winning Connection", SetWinningConnection));
        Options.Add(new MenuOption('S', "Start Game with These Settings", StartGame));
        Options.Add(new MenuOption('B', "Back to Main Menu", Back));
    }

    private void SelectBoardShape()
    {
        Console.Clear();
        Console.WriteLine("=== SELECT BOARD SHAPE ===");
        Console.WriteLine();
        Console.WriteLine($"Current shape: {_settings.BoardShape}");
        Console.WriteLine();
        Console.WriteLine("1. Rectangle (standard)");
        Console.WriteLine("2. Cylinder (edges wrap around)");
        Console.Write("\nChoice: ");

        var choice = Console.ReadKey(true).KeyChar;

        _settings.BoardShape = choice switch
        {
            '1' => "Rectangle",
            '2' => "Cylinder",
            _ => _settings.BoardShape
        };

        Console.WriteLine($"\n\nBoard shape set to: {_settings.BoardShape}");
        Console.ReadKey();
    }

    private void SetBoardSize()
    {
        Console.Clear();
        Console.WriteLine("=== SET BOARD SIZE ===");
        Console.WriteLine();
        Console.WriteLine($"Current size: {_settings.BoardHeight}h x {_settings.BoardWidth}w");
        Console.WriteLine();

        Console.Write($"Enter board height (min: {MinBoardDimension}, max: {MaxBoardDimension}): ");
        if (int.TryParse(Console.ReadLine(), out int height) && height >= MinBoardDimension && height <= MaxBoardDimension)
        {
            _settings.BoardHeight = height;
        }
        else
        {
            Console.WriteLine($"Invalid height. Keeping current: {_settings.BoardHeight}");
        }

        Console.Write($"Enter board width (min: {MinBoardDimension}, max: {MaxBoardDimension}): ");
        if (int.TryParse(Console.ReadLine(), out int width) && width >= MinBoardDimension && width <= MaxBoardDimension)
        {
            _settings.BoardWidth = width;
        }
        else
        {
            Console.WriteLine($"Invalid width. Keeping current: {_settings.BoardWidth}");
        }

        Console.WriteLine($"\nBoard size set to: {_settings.BoardHeight}h x {_settings.BoardWidth}w");
        Console.ReadKey();
    }

    private void SetWinningConnection()
    {
        Console.Clear();
        Console.WriteLine("=== SET WINNING CONNECTION ===");
        Console.WriteLine();
        Console.WriteLine($"Current winning connection: {_settings.WinningCondition}");
        Console.WriteLine($"Board dimensions: {_settings.BoardHeight}h x {_settings.BoardWidth}w");

        int maxWin = Math.Max(_settings.BoardHeight, _settings.BoardWidth);

        Console.Write($"\nEnter winning connection (min: {MinWinCondition}, max: {maxWin}): ");

        if (int.TryParse(Console.ReadLine(), out int winCondition) && winCondition >= MinWinCondition && winCondition <= maxWin)
        {
            _settings.WinningCondition = winCondition;
            Console.WriteLine($"\nWinning connection set to: {_settings.WinningCondition}");
        }
        else
        {
            Console.WriteLine($"\nInvalid connection. Must be between {MinWinCondition} and {maxWin}.");
        }

        Console.ReadKey();
    }

    private void StartGame()
    {
        _settings.GameMode = "Custom";

        Console.Clear();
        var random = new Random();
        var startingPlayer = random.Next(2) == 0 ? _settings.Player1Name : _settings.Player2Name;
        _settings.StartingPlayerName = startingPlayer;

        Console.WriteLine("=== GAME STARTING ===");
        Console.WriteLine();
        Console.WriteLine($"Player 1: {_settings.Player1Name}");
        Console.WriteLine($"Player 2: {_settings.Player2Name}");
        Console.WriteLine($"Board Shape: {_settings.BoardShape}");
        Console.WriteLine($"Board Size: {_settings.BoardHeight}h x {_settings.BoardWidth}w");
        Console.WriteLine($"Win Condition: {_settings.WinningCondition} in a row");
        Console.WriteLine();
        Console.WriteLine($"{startingPlayer} will start first!");
        Console.WriteLine();
        Console.WriteLine("Press any key to start...");
        Console.ReadKey();

        GameController.StartGame();

        if (_menuManager != null)
        {
            _menuManager.NavigateTo(EMenuType.Main);
        }
    }
}
