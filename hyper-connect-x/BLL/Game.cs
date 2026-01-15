namespace BLL;

public class Game
{
    public Board Board { get; }
    public Player Player1 { get; }
    public Player Player2 { get; }
    public Player CurrentPlayer { get; internal set; }
    public bool IsGameOver { get; internal set; }
    public Player? Winner { get; internal set; }
    public int WinningCondition { get; }
    public bool IsDraw => Board.IsFull && Winner == null;

    public Game(Player player1, Player player2, int height, int width, string shape, int winningCondition,
        Player startingPlayer)
    {
        Player1 = player1;
        Player2 = player2;
        WinningCondition = winningCondition;
        Board = new Board(height, width, shape);
        CurrentPlayer = startingPlayer;
    }

    public bool MakeMove(int column)
    {
        if (IsGameOver)
        {
            return false;
        }

        if (column < 0 || column >= Board.Width)
        {
            return false;
        }

        bool placed = Board.PlacePiece(column, CurrentPlayer);
        if (!placed)
        {
            return false;
        }

        int row = FindPieceRow(column);

        if (CheckWin(row, column))
        {
            IsGameOver = true;
            Winner = CurrentPlayer;
            return true;
        }

        if (Board.IsFull)
        {
            IsGameOver = true;
            Winner = null;
            return true;
        }

        CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        return true;
    }

    public Player GetOpponent(Player player)
    {
        return player == Player1 ? Player2 : Player1;
    }

    public bool WouldWin(int row, int column, Player player)
    {
        int horizontal = 1 + CountDirection(row, column, 0, -1, player) +
                         CountDirection(row, column, 0, 1, player);

        if (horizontal >= WinningCondition)
        {
            return true;
        }

        int vertical = 1 + CountDirection(row, column, -1, 0, player) +
                       CountDirection(row, column, 1, 0, player);

        if (vertical >= WinningCondition)
        {
            return true;
        }

        int diagonal1 = 1 + CountDirection(row, column, -1, -1, player) +
                        CountDirection(row, column, 1, 1, player);
        if (diagonal1 >= WinningCondition)
        {
            return true;
        }

        int diagonal2 = 1 + CountDirection(row, column, -1, 1, player) +
                        CountDirection(row, column, 1, -1, player);

        if (diagonal2 >= WinningCondition)
        {
            return true;
        }

        return false;
    }

    private bool CheckWin(int lastRow, int lastColumn)                                                                                                                                         
    {
        return WouldWin(lastRow, lastColumn, CurrentPlayer);
    }


    private int CountDirection(int row, int column, int dRow, int dColumn, Player player)
    {
        int count = 0;
        row += dRow;
        column += dColumn;

        while (true)
        {
            Player? cell = Board.GetCell(row, column);
            if (cell != player)
            {
                break;
            }

            count++;

            row += dRow;
            column += dColumn;
        }

        return count;
    }

    private int FindPieceRow(int column)
    {
        for (int row = 0; row < Board.Height; row++)
        {
            if (Board.GetCell(row, column) == CurrentPlayer)
            {
                return row;
            }
        }

        return -1;
    }
    
    
}