namespace BLL;

public static class GameConfiguration
{
    public const int MinBoardSize = 3;
    public const int MaxBoardSize = 20;
    public const int MinWinCondition = 2;

    public const int DefaultBoardHeight = 6;
    public const int DefaultBoardWidth = 7;
    public const int DefaultWinCondition = 4;

    public const int ThreePiecesThreshold = 3;
    public const int TwoPiecesThreshold = 2;
    public const int OnePieceThreshold = 1;

    public const int WebAiDelayMs = 1000;

    public static readonly ConsoleColor DefaultPlayer1Color = ConsoleColor.Red;
    public static readonly ConsoleColor DefaultPlayer2Color = ConsoleColor.Yellow;
}
