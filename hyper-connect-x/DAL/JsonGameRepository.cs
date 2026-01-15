using System.Text.Json;

namespace DAL;

public class JsonGameRepository : IGameRepository
{
    private readonly string _saveDirectory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonGameRepository(string saveDirectory = "saves")
    {
        _saveDirectory = saveDirectory;
        if (!Directory.Exists(_saveDirectory))
        {
            Directory.CreateDirectory(_saveDirectory);
        }

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
    }

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

        string filePath = GetFilePath(gameState.GameId);
        string json = JsonSerializer.Serialize(gameState, _jsonSerializerOptions);
        File.WriteAllText(filePath, json);

        return gameState.GameId;
    }

    public GameState? LoadGame(string gameId)
    {
        string filePath = GetFilePath(gameId);
        if (!File.Exists(filePath))
        {
            return null;
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<GameState>(json);
    }

    public List<GameState> GetAllGames()
    {
        var games = new List<GameState>();
        var files = Directory.GetFiles(_saveDirectory, "*.json");

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            var gameState = JsonSerializer.Deserialize<GameState>(json);
            if (gameState != null)
            {
                games.Add(gameState);
            }
        }

        return games.OrderByDescending(g => g.UpdatedAt ?? g.CreatedAt).ToList();
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
        string filePath = GetFilePath(gameId);
        if (!File.Exists(filePath))
        {
            return false;
        }

        File.Delete(filePath);
        return true;
    }

    public bool GameExists(string gameId)
    {
        return File.Exists(GetFilePath(gameId));
    }


    private string GetFilePath(string gameId)
    {
        return Path.Combine(_saveDirectory, gameId + ".json");
    }
}