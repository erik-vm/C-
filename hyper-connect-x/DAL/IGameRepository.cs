namespace DAL;

public interface IGameRepository
{
    string SaveGame(GameState gameState);
    GameState? LoadGame(string gameId);
    List<GameState> GetAllGames();
    bool UpdateGame(string gameId, GameState gameState);
    bool DeleteGame(string gameId);
    bool GameExists(string gameId);
}