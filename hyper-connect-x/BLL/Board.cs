namespace BLL;

public class Board
{
    public int Height { get; }
    public int Width { get; }
    public string Shape { get; }
    private Player?[,] _cells;

    public bool IsFull
    {
        get
        {
            for (int col = 0; col < Width; col++)
            {
                if (_cells[0, col] == null)
                {
                    return false;
                }
            }

            return true;
        }
    }


    public Board(int height, int width, string shape)
    {
        Height = height;
        Width = width;
        Shape = shape;
        _cells = new Player[height, width];
    }

    public bool IsColumnFull(int column)
    {
        return _cells[0, column] != null;
    }

    public bool PlacePiece(int column, Player player)
    {
        int row = GetLowestEmptyRow(column);
        if (row == -1) return false;
        _cells[row, column] = player;
        return true;
    }

    public void SetCell(int row, int column, Player? player)
    {
        if (row >= 0 && row < Height && column >= 0 && column < Width)
        {
            _cells[row, column] = player;
        } 
    }

    public Player? GetCell(int row, int column)
    {
        if (Shape == "Cylinder")
        {
            column = ((column % Width) + Width) % Width;
        }

        if (row < 0 || row >= Height || column < 0 || column >= Width)
        {
            return null;
        }

        return _cells[row, column];
    }

    public int GetLowestEmptyRow(int column)
    {
        for (int row = Height - 1; row >= 0; row--)
        {
            if (_cells[row, column] == null)
            {
                return row;
            }
        }

        return -1;
    }
}