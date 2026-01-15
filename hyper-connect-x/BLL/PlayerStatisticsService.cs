using DAL;

namespace BLL;

public class PlayerStatisticsService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerStatisticsService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public void RecordGameCompletion(Game game)
    {
        if (game == null || !game.IsGameOver)
        {
            return;
        }

        UpdatePlayerStatistic(game.Player1.Name, game.Winner?.Name);
        UpdatePlayerStatistic(game.Player2.Name, game.Winner?.Name);
    }

    private void UpdatePlayerStatistic(string playerName, string? winnerName)
    {
        var profile = _playerRepository.GetPlayer(playerName);
        if (profile == null)
        {
            return;
        }

        profile.TotalGamesPlayed++;
        if (winnerName == playerName)
        {
            profile.TotalWins++;
        }
        _playerRepository.SavePlayer(profile);
    }
}
