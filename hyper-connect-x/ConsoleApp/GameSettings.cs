namespace ConsoleApp;

public class GameSettings
{
    public string Player1Name { get; set; } = "Deep Blue";
    public ConsoleColor Player1Color { get; set; } = ConsoleColor.Red;
    public string Player2Name { get; set; } = "Watson";
    public ConsoleColor Player2Color { get; set; } = ConsoleColor.Yellow;
    public int BoardHeight { get; set; } = 6;
    public int BoardWidth { get; set; } = 7;
    public int WinningCondition { get; set; } = 4;
    public string GameMode { get; set; } = "Classical";
    public string BoardShape { get; set; } = "Rectangle";
    public string Player1AiDifficulty { get; set; } = "Medium";
    public string Player2AiDifficulty { get; set; } = "Medium";

    public bool Player1IsAi { get; set; }
    public bool Player2IsAi { get; set; }

    public string StartingPlayerName { get; set; } = "";
    public string RepositoryType { get; set; } = "json";

    private static GameSettings? _instance;

    public static GameSettings Instance => _instance ??= new GameSettings();

    private GameSettings()
    {
    }
}