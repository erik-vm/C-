using BLL.AI;

namespace BLL;

public class Player
{
    public string Name { get; }
    public ConsoleColor Color { get; }
    public bool IsAi { get; }
    public string? AiDifficulty { get; }

    private readonly IAiStrategy? _aiStrategy;

    public Player(string name, ConsoleColor color, bool isAi, string? aiDifficulty = null)
    {
        Name = name;
        Color = color;
        IsAi = isAi;
        AiDifficulty = aiDifficulty;

        if (isAi)
        {
            _aiStrategy = AiStrategyFactory.Create(aiDifficulty);
        }
    }

    public int GetAiMove(Game game)
    {
        if (!IsAi || _aiStrategy == null)
            throw new InvalidOperationException("Not an AI player");

        return _aiStrategy.GetMove(game, this);
    }
}
