namespace BLL.AI;

public interface IAiStrategy
{
    int GetMove(Game game, Player aiPlayer);
}