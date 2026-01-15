namespace ConsoleApp.UI;

public static class ColorHelper
{
    public static void WriteColorBlock(ConsoleColor color)
    {
        var originalBg = Console.BackgroundColor;
        var originalFg = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.Write("███");

        Console.BackgroundColor = originalBg;
        Console.ForegroundColor = originalFg;
    }

    public static void WriteColored(string text, ConsoleColor color)
    {
        var originalFg = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = originalFg;
    }
}