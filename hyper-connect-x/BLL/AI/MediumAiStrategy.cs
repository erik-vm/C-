namespace BLL.AI;

public class MediumAiStrategy : IAiStrategy
{
    public int GetMove(Game game, Player aiPlayer)
    {
        int winningMove = BoardEvaluator.FindWinningMove(game, aiPlayer);
        if (winningMove != -1) return winningMove;

        Player opponent = game.GetOpponent(aiPlayer);
        int blockingMove = BoardEvaluator.FindWinningMove(game, opponent);
        if (blockingMove != -1) return blockingMove;

        int centerMove = BoardEvaluator.GetCenterMove(game);
        if (centerMove != -1) return centerMove;

        return AiHelper.GetRandomValidMove(game);
    }
}