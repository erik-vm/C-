namespace DAL;

public static class RepositoryFactory
{
    public static IGameRepository CreateRepository(string type, GameDbContext? dbContext = null)
    {
        return type.ToLower() switch
        {
            "json" => new JsonGameRepository(),
            "database" => new EfGameRepository(dbContext ?? throw new ArgumentNullException(nameof(dbContext), "Database context is required for database repository")),
            _ => throw new ArgumentException($"Unknown repository type: {type}")
        };
    }
}