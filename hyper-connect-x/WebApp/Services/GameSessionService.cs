using BLL;
using DAL;

namespace WebApp.Services;

public class GameSessionService
{
    public static void SaveGameToSession(ISession session, Game game, int moveCount, string? gameId = null)
    {
        var gameState = GameStateConverter.ToGameState(game, gameId ?? "temp-session");

        var sessionData = new GameSessionData
        {
            GameState = gameState,
            MoveCount = moveCount
        };

        var json = System.Text.Json.JsonSerializer.Serialize(sessionData);
        session.SetString("CurrentGameJson", json);
    }

    public static (Game? game, int moveCount, string? gameId) LoadGameFromSession(ISession session)
    {
        var sessionData = session.GetString("CurrentGameJson");

        if (string.IsNullOrEmpty(sessionData))
        {
            return (null, 0, null);
        }

        try
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<GameSessionData>(sessionData);
            if (data?.GameState != null)
            {
                var game = GameStateConverter.ToGame(data.GameState);
                return (game, data.MoveCount, data.GameState.GameId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Session deserialization failed: {ex.Message}");
        }

        return (null, 0, null);
    }

    private class GameSessionData
    {
        public GameState GameState { get; set; } = null!;
        public int MoveCount { get; set; }
    }
}
