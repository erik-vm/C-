namespace BLL.AI;

public class EasyAiStrategy : IAiStrategy
{
    public int GetMove(Game game, Player aiPlayer)
    {
        return AiHelper.GetRandomValidMove(game);
    }
}