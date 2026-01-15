using DAL.Models;

namespace DAL;

public class EfPlayerRepository : IPlayerRepository
{
    private readonly GameDbContext _context;

    public EfPlayerRepository(GameDbContext context)
    {
        _context = context;
    }

    public List<PlayerProfile> GetAllPlayers()
    {
        return _context.PlayerProfiles
            .Select(p => MapToProfile(p))
            .ToList();
    }

    public PlayerProfile? GetPlayer(string playerName)
    {
        var entity = _context.PlayerProfiles.Find(playerName);
        return entity == null ? null : MapToProfile(entity);
    }

    public void SavePlayer(PlayerProfile player)
    {
        var entity = _context.PlayerProfiles.Find(player.PlayerName);

        if (entity == null)
        {
            entity = MapToEntity(player);
            _context.PlayerProfiles.Add(entity);
        }
        else
        {
            entity.TotalGamesPlayed = player.TotalGamesPlayed;
            entity.TotalWins = player.TotalWins;
        }

        _context.SaveChanges();
    }

    private static PlayerProfile MapToProfile(PlayerProfileEntity entity)
    {
        return new PlayerProfile
        {
            PlayerName = entity.PlayerName,
            IsAi = entity.IsAi,
            AiDifficulty = entity.AiDifficulty,
            CreatedAt = entity.CreatedAt,
            TotalGamesPlayed = entity.TotalGamesPlayed,
            TotalWins = entity.TotalWins
        };
    }

    private static PlayerProfileEntity MapToEntity(PlayerProfile profile)
    {
        return new PlayerProfileEntity
        {
            PlayerName = profile.PlayerName,
            IsAi = profile.IsAi,
            AiDifficulty = profile.AiDifficulty,
            CreatedAt = profile.CreatedAt,
            TotalGamesPlayed = profile.TotalGamesPlayed,
            TotalWins = profile.TotalWins
        };
    }

    public void DeletePlayer(string playerName)
    {
        var entity = _context.PlayerProfiles.Find(playerName);
        if (entity != null)
        {
            _context.PlayerProfiles.Remove(entity);
            _context.SaveChanges();
        }
    }
}
