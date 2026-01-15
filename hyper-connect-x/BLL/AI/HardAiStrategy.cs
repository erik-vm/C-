namespace BLL.AI;

public class HardAiStrategy : IAiStrategy
{
    private const int SmallBoardSize = 49;
    private const int MediumBoardSize = 100;
    private const int SmallBoardDepth = 6;
    private const int MediumBoardDepth = 4;
    private const int LargeBoardDepth = 3;
    private const int WinScore = 10000;

    public int GetMove(Game game, Player aiPlayer)
    {
        int boardSize = game.Board.Width * game.Board.Height;
        int depth = boardSize switch
        {
            <= SmallBoardSize => SmallBoardDepth,
            <= MediumBoardSize => MediumBoardDepth,
            _ => LargeBoardDepth
        };

        int bestScore = int.MinValue;
        int bestColumn = -1;

        for (int col = 0; col < game.Board.Width; col++)
        {
            if (game.Board.IsColumnFull(col)) continue;

            int row = game.Board.GetLowestEmptyRow(col);
            if (row == -1) continue;

            game.Board.SetCell(row, col, aiPlayer);

            int score = Minimax(game, aiPlayer, depth - 1, false, int.MinValue, int.MaxValue);

            game.Board.SetCell(row, col, null);

            if (score > bestScore)
            {
                bestScore = score;
                bestColumn = col;
            }
        }

        return bestColumn != -1 ? bestColumn : AiHelper.GetRandomValidMove(game);
    }

    private int Minimax(Game game, Player aiPlayer, int depth, bool isMaximizing, int alpha, int beta)
    {
        if (depth == 0)
        {
            return BoardEvaluator.EvaluateBoard(game, aiPlayer);
        }

        if (game.Board.IsFull)
        {
            return 0;
        }

        Player currentPlayer = isMaximizing ? aiPlayer : game.GetOpponent(aiPlayer);
        int bestEval = isMaximizing ? int.MinValue : int.MaxValue;

        for (int col = 0; col < game.Board.Width; col++)
        {
            int? evaluation = TryEvaluateMove(game, aiPlayer, depth, isMaximizing, currentPlayer, col, ref alpha, ref beta);

            if (evaluation.HasValue)
            {
                return evaluation.Value;
            }

            bestEval = isMaximizing ? Math.Max(bestEval, alpha) : Math.Min(bestEval, beta);

            if (beta <= alpha)
            {
                break;
            }
        }

        return bestEval;
    }

    private int? TryEvaluateMove(Game game, Player aiPlayer, int depth, bool isMaximizing, Player currentPlayer, int col, ref int alpha, ref int beta)
    {
        if (game.Board.IsColumnFull(col))
        {
            return null;
        }

        int row = game.Board.GetLowestEmptyRow(col);
        if (row == -1)
        {
            return null;
        }

        game.Board.SetCell(row, col, currentPlayer);

        if (game.WouldWin(row, col, currentPlayer))
        {
            game.Board.SetCell(row, col, null);
            return isMaximizing ? WinScore - depth : -WinScore + depth;
        }

        int eval = Minimax(game, aiPlayer, depth - 1, !isMaximizing, alpha, beta);
        game.Board.SetCell(row, col, null);

        if (isMaximizing)
        {
            alpha = Math.Max(alpha, eval);
        }
        else
        {
            beta = Math.Min(beta, eval);
        }

        return null;
    }
}