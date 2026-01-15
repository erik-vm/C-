using Microsoft.EntityFrameworkCore;

namespace DAL;

public class EfGameRepository(GameDbContext context) : IGameRepository
{
    public string SaveGame(GameState gameState)
    {
        if (string.IsNullOrEmpty(gameState.GameId))
        {
            gameState.GameId = Guid.NewGuid().ToString();
            gameState.CreatedAt = DateTime.Now;
        }
        else
        {
            gameState.UpdatedAt = DateTime.Now;
        }

        var entity = EntityConverter.ToEntity(gameState);

        var existing = context.GameStates
            .Include(g => g.BoardCells)
            .FirstOrDefault(g => g.GameId == gameState.GameId);

        if (existing != null)
        {
            context.BoardCells.RemoveRange(existing.BoardCells);
            context.GameStates.Remove(existing);
        }

        context.GameStates.Add(entity);
        context.SaveChanges();

        return gameState.GameId;
    }

    public GameState? LoadGame(string gameId)
    {
        var entity = context.GameStates
            .Include(g => g.BoardCells)
            .FirstOrDefault(g => g.GameId == gameId);

        if (entity == null)
        {
            return null;
        }

        return EntityConverter.ToGameState(entity);
    }

    public List<GameState> GetAllGames()
    {
        var entities = context.GameStates
            .Include(g => g.BoardCells)
            .OrderByDescending(g => g.UpdatedAt ?? g.CreatedAt)
            .ToList();

        return entities.Select(EntityConverter.ToGameState).ToList();
    }

    public bool UpdateGame(string gameId, GameState gameState)
    {
        if (!GameExists(gameId))
        {
            return false;
        }

        gameState.GameId = gameId;
        SaveGame(gameState);
        return true;
    }

    public bool DeleteGame(string gameId)
    {
        var entity = context.GameStates.Find(gameId);
        if (entity == null)
        {
            return false;
        }

        context.GameStates.Remove(entity);
        context.SaveChanges();
        return true;
    }

    public bool GameExists(string gameId)
    {
        return context.GameStates.Any(g => g.GameId == gameId);
    }
}