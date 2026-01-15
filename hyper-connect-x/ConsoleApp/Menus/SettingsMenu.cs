using ConsoleApp.UI;
using static System.ConsoleColor;

namespace ConsoleApp.Menus;

public class SettingsMenu : BaseMenu
{
    private readonly GameSettings _settings;
    private bool _needsRefresh = false;

    public SettingsMenu() : base("SETTINGS", EMenuType.Settings)
    {
        _settings = GameSettings.Instance;
        InitializeOptions();
    }

    protected override void Display()
    {
        // Only refresh if something changed
        if (_needsRefresh)
        {
            Options.Clear();
            InitializeOptions();
            _needsRefresh = false;
        }

        Console.Clear();
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine(Title);
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();

        for (int i = 0; i < Options.Count; i++)
        {
            if (i == SelectedIndex)
            {
                Console.BackgroundColor = Gray;
                Console.ForegroundColor = Black;
                Console.Write($"> [{Options[i].Shortcut}] ");
            }
            else
            {
                Console.Write($"  [{Options[i].Shortcut}] ");
            }

            // Special rendering for color options
            if (i == 1) // Player 1 Color
            {
                Console.Write($"Change Player 1 Color (Current: ");
                if (i == SelectedIndex) Console.ResetColor();
                ColorHelper.WriteColorBlock(_settings.Player1Color);
                if (i == SelectedIndex)
                {
                    Console.BackgroundColor = Gray;
                    Console.ForegroundColor = Black;
                }

                Console.Write($" {_settings.Player1Color})");
            }
            else if (i == 3) // Player 2 Color
            {
                Console.Write($"Change Player 2 Color (Current: ");
                if (i == SelectedIndex) Console.ResetColor();
                ColorHelper.WriteColorBlock(_settings.Player2Color);
                if (i == SelectedIndex)
                {
                    Console.BackgroundColor = Gray;
                    Console.ForegroundColor = Black;
                }

                Console.Write($" {_settings.Player2Color})");
            }
            else
            {
                Console.Write(Options[i].DisplayText);
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine("Use ↑↓ arrows to navigate, Enter to select, or press shortcut key");
    }

    protected sealed override void InitializeOptions()
    {
        Options.Add(new MenuOption('1', $"Change Player 1 Name (Current: {_settings.Player1Name})", ChangePlayer1Name));
        Options.Add(new MenuOption('2', "Player 1 Color", ChangePlayer1Color));
        Options.Add(new MenuOption('3', $"Change Player 2 Name (Current: {_settings.Player2Name})", ChangePlayer2Name));
        Options.Add(new MenuOption('4', "Player 2 Color", ChangePlayer2Color));
        Options.Add(new MenuOption('5', $"Change Board Size (Current: {_settings.BoardHeight}x{_settings.BoardWidth})",
            ChangeBoardSize));
        Options.Add(new MenuOption('6', $"Change Game Mode (Current: {_settings.GameMode})", ChangeGameMode));
        Options.Add(
            new MenuOption('7', $"Change Player 1 AI Difficulty (Current: {_settings.Player1AiDifficulty})",
                ChangePlayer1AIDifficulty));
        Options.Add(
            new MenuOption('8', $"Change Player 2 AI Difficulty (Current: {_settings.Player2AiDifficulty})",
                ChangePlayer2AIDifficulty));
        Options.Add(new MenuOption('9', $"Storage Type (Current: {_settings.RepositoryType})", ChangeStorageType));
        Options.Add(new MenuOption('B', "Back", Back));
        Options.Add(new MenuOption('X', "Exit", Exit));
    }

    private void ChangeName(string playerLabel, ref string currentName)
    {
        Console.Write($"Enter {playerLabel} name (current: {currentName}): ");
        var name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name))
        {
            currentName = name;
            Console.WriteLine($"{playerLabel} name changed to: {name}");
        }
        else
        {
            Console.WriteLine("Name unchanged.");
        }

        Console.ReadKey();
        _needsRefresh = true;
    }

    private void ChangeColor(string playerLabel, ref ConsoleColor currentColor)
    {
        Console.WriteLine($"Select {playerLabel} color (current: {currentColor}):");
        ColorHelper.WriteColored("1. Red\n", Red);
        ColorHelper.WriteColored("2. Yellow\n", Yellow);
        ColorHelper.WriteColored("3. Green\n", Green);
        ColorHelper.WriteColored("4. Blue\n", Blue);
        ColorHelper.WriteColored("5. Cyan\n", Cyan);
        ColorHelper.WriteColored("6. Magenta\n", Magenta);
        Console.Write("Choice: ");

        var choice = Console.ReadKey(true).KeyChar;

        currentColor = choice switch
        {
            '1' => Red,
            '2' => Yellow,
            '3' => Green,
            '4' => Blue,
            '5' => Cyan,
            '6' => Magenta,
            _ => currentColor
        };

        Console.WriteLine($"\n{playerLabel} color changed to: {currentColor}");
        Console.ReadKey();
        _needsRefresh = true;
    }

    private void ChangePlayer1Name()
    {
        string name = _settings.Player1Name;
        ChangeName("Player 1", ref name);
        _settings.Player1Name = name;
    }

    private void ChangePlayer1Color()
    {
        ConsoleColor color = _settings.Player1Color;
        ChangeColor("Player 1", ref color);
        _settings.Player1Color = color;
    }

    private void ChangePlayer2Name()
    {
        string name = _settings.Player2Name;
        ChangeName("Player 2", ref name);
        _settings.Player2Name = name;
    }

    private void ChangePlayer2Color()
    {
        ConsoleColor color = _settings.Player2Color;
        ChangeColor("Player 2", ref color);
        _settings.Player2Color = color;
    }

    private void ChangeBoardSize()
    {
        Console.Write($"Enter board height (current: {_settings.BoardHeight}): ");
        if (int.TryParse(Console.ReadLine(), out int height) && height > 0)
            _settings.BoardHeight = height;

        Console.Write($"Enter board width (current: {_settings.BoardWidth}): ");
        if (int.TryParse(Console.ReadLine(), out int width) && width > 0)
            _settings.BoardWidth = width;

        Console.WriteLine($"Board size changed to: {_settings.BoardHeight}x{_settings.BoardWidth}");
        Console.ReadKey();
        _needsRefresh = true;
    }

    private void ChangeGameMode()
    {
        Console.WriteLine($"Select game mode (current: {_settings.GameMode}):");
        Console.WriteLine("1. Classical (6x7, win:4)");
        Console.WriteLine("2. Custom");
        Console.Write("Choice: ");

        var choice = Console.ReadKey(true).KeyChar;
        _settings.GameMode = choice == '1' ? "Classical" : "Custom";

        Console.WriteLine($"\nGame mode changed to: {_settings.GameMode}");
        Console.ReadKey();
        _needsRefresh = true;
    }

    private void ChangePlayer1AIDifficulty()
    {
        Console.WriteLine($"Select Player 1 AI difficulty (current: {_settings.Player1AiDifficulty}):");
        Console.WriteLine("1. Easy");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. Hard");
        Console.Write("Choice: ");

        var choice = Console.ReadKey(true).KeyChar;
        _settings.Player1AiDifficulty = choice switch
        {
            '1' => "Easy",
            '2' => "Medium",
            '3' => "Hard",
            _ => _settings.Player1AiDifficulty
        };

        Console.WriteLine($"\nPlayer 1 AI difficulty changed to: {_settings.Player1AiDifficulty}");
        Console.ReadKey();
        _needsRefresh = true;
    }

    private void ChangePlayer2AIDifficulty()
    {
        Console.WriteLine($"Select Player 2 AI difficulty (current: {_settings.Player2AiDifficulty}):");
        Console.WriteLine("1. Easy");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. Hard");
        Console.Write("Choice: ");

        var choice = Console.ReadKey(true).KeyChar;
        _settings.Player2AiDifficulty = choice switch
        {
            '1' => "Easy",
            '2' => "Medium",
            '3' => "Hard",
            _ => _settings.Player2AiDifficulty
        };

        Console.WriteLine($"\nPlayer 2 AI difficulty changed to: {_settings.Player2AiDifficulty}");
        Console.ReadKey();
        _needsRefresh = true;
    }

    private void ChangeStorageType()
    {
        Console.Clear();
        Console.WriteLine("=== CHANGE STORAGE TYPE ===\n");
        Console.WriteLine($"Current: {_settings.RepositoryType}\n");
        Console.WriteLine("1. JSON Files");
        Console.WriteLine("2. Database (SQLite)");
        Console.Write("\nChoice: ");

        var choice = Console.ReadKey(true).KeyChar;

        _settings.RepositoryType = choice switch
        {
            '1' => "json",
            '2' => "database",
            _ => _settings.RepositoryType
        };

        Console.WriteLine($"\n\nStorage type set to: {_settings.RepositoryType}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        _needsRefresh = true;
    }
}