namespace DAL;

public interface IPlayerRepository
{
    List<PlayerProfile> GetAllPlayers();
    PlayerProfile? GetPlayer(string playerName);
    void SavePlayer(PlayerProfile player);
    void DeletePlayer(string playerName);
}
