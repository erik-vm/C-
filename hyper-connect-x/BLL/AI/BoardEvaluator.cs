namespace BLL.AI;

public static class BoardEvaluator
{
    private const int CenterPositionScore = 3;
    private const int ThreeInLineScore = 100;
    private const int TwoInLineScore = 10;
    private const int OneInLineScore = 1;

    private static int ThreePiecesThreshold => GameConfiguration.ThreePiecesThreshold;
    private static int TwoPiecesThreshold => GameConfiguration.TwoPiecesThreshold;
    private static int OnePieceThreshold => GameConfiguration.OnePieceThreshold;

    public static int FindWinningMove(Game game, Player player)
    {
        for (int col = 0; col < game.Board.Width; col++)
        {
            if (game.Board.IsColumnFull(col)) continue;

            int row = game.Board.GetLowestEmptyRow(col);
            if (row == -1) continue;

            game.Board.SetCell(row, col, player);
            bool isWin = game.WouldWin(row, col, player);
            game.Board.SetCell(row, col, null);

            if (isWin) return col;
        }
        return -1;
    }

    public static int GetCenterMove(Game game)
    {
        int center = game.Board.Width / 2;

        if (!game.Board.IsColumnFull(center))
            return center;

        if (center - 1 >= 0 && !game.Board.IsColumnFull(center - 1))
            return center - 1;

        if (center + 1 < game.Board.Width && !game.Board.IsColumnFull(center + 1))
            return center + 1;

        return -1;
    }

    public static int EvaluateBoard(Game game, Player aiPlayer)
    {
        int score = 0;

        int centerCol = game.Board.Width / 2;
        for (int row = 0; row < game.Board.Height; row++)
        {
            Player? cell = game.Board.GetCell(row, centerCol);
            if (cell == aiPlayer)
                score += CenterPositionScore;
            else if (cell != null)
                score -= CenterPositionScore;
        }

        Player opponent = game.GetOpponent(aiPlayer);
        score += EvaluateLines(game, aiPlayer) - EvaluateLines(game, opponent);

        return score;
    }

    private static int EvaluateLines(Game game, Player player)
    {
        int score = 0;

        for (int row = 0; row < game.Board.Height; row++)
        {
            for (int col = 0; col < game.Board.Width; col++)
            {
                score += EvaluatePosition(game, row, col, player);
            }
        }

        return score;
    }

    private static int EvaluatePosition(Game game, int row, int col, Player player)
    {
        int score = 0;
        int winCondition = game.WinningCondition;

        int[] dRows = { 0, 1, 1, 1 };
        int[] dCols = { 1, 0, 1, -1 };

        for (int dir = 0; dir < 4; dir++)
        {
            int count = 0;
            int empty = 0;

            for (int i = 0; i < winCondition; i++)
            {
                int r = row + dRows[dir] * i;
                int c = col + dCols[dir] * i;

                Player? cell = game.Board.GetCell(r, c);

                if (cell == player)
                    count++;
                else if (cell == null)
                    empty++;
                else
                    break;
            }

            if (count + empty == winCondition)
            {
                if (count == ThreePiecesThreshold)
                    score += ThreeInLineScore;
                else if (count == TwoPiecesThreshold)
                    score += TwoInLineScore;
                else if (count == OnePieceThreshold)
                    score += OneInLineScore;
            }
        }

        return score;
    }
}