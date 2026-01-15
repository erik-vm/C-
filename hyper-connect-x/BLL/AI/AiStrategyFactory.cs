namespace BLL.AI;

public static class AiStrategyFactory
{
    public static IAiStrategy Create(string? difficulty)
    {
        return difficulty?.ToLower() switch
        {
            "easy" => new EasyAiStrategy(),
            "medium" => new MediumAiStrategy(),
            "hard" => new HardAiStrategy(),
            _ => new EasyAiStrategy()
        };
    }
}