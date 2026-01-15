using BLL;

namespace ConsoleApp;

public abstract class GameRenderer
{
    public static void RenderBoard(Game game)
    {
        Board board = game.Board;

        Console.Write("  ");
        for (int col = 0; col < board.Width; col++)
        {
            Console.Write($"  {col + 1} ");
        }

        Console.WriteLine();
        PrintHorizontalBorder(board.Width, isTop: true);

        for (int row = 0; row < board.Height; row++)
        {
            Console.Write("  │");
            for (int col = 0; col < board.Width; col++)
            {
                Player? cell = board.GetCell(row, col);
                if (cell == null)
                {
                    Console.Write("   ");
                }
                else
                {
                    Console.Write(" ");
                    Console.ForegroundColor = cell.Color;
                    Console.Write("●");
                    Console.ResetColor();
                    Console.Write(" ");
                }

                Console.Write("│");
            }

            Console.WriteLine();

            if (row < board.Height - 1)
            {
                PrintHorizontalBorder(board.Width, isTop: false);
            }
        }

        PrintHorizontalBorder(board.Width, isBottom: true);
    }

    private static void PrintHorizontalBorder(int width, bool isTop = false, bool isBottom = false)
    {
        Console.Write("  ");
        if (isTop)
        {
            Console.Write("┌");
            for (int i = 0; i < width; i++)
            {
                Console.Write("───");
                Console.Write(i < width - 1 ? "┬" : "┐");
            }
        }
        else if (isBottom)
        {
            Console.Write("└");
            for (int i = 0; i < width; i++)
            {
                Console.Write("───");
                Console.Write(i < width - 1 ? "┴" : "┘");
            }
        }
        else // Middle separator                                                                                                                                                                                                          
        {
            Console.Write("├");
            for (int i = 0; i < width; i++)
            {
                Console.Write("───");
                Console.Write(i < width - 1 ? "┼" : "┤");
            }
        }

        Console.WriteLine();
    }
}