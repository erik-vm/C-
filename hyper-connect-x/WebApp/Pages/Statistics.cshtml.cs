using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class StatisticsModel : PageModel
{
    private readonly IGameRepository _repository;
    private readonly IPlayerRepository _playerRepository;

    public StatisticsModel(IGameRepository repository, IPlayerRepository playerRepository)
    {
        _repository = repository;
        _playerRepository = playerRepository;
    }

    public List<GameState> AllGames { get; set; } = new();
    public List<PlayerStat> PlayerStats { get; set; } = new();
    public int CompletedGames { get; set; }
    public int InProgressGames { get; set; }
    public double AverageMovesPerGame { get; set; }

    public void OnGet()
    {
        AllGames = _repository.GetAllGames();

        if (AllGames.Count > 0)
        {
            CalculateStatistics();
        }

        LoadPlayerProfiles();
    }

    private void LoadPlayerProfiles()
    {
        var allPlayers = _playerRepository.GetAllPlayers();
        PlayerStats = allPlayers
            .Select(p => new PlayerStat
            {
                PlayerName = p.PlayerName,
                GamesPlayed = p.TotalGamesPlayed,
                Wins = p.TotalWins,
                IsAi = p.IsAi,
                AiDifficulty = p.AiDifficulty
            })
            .OrderByDescending(p => p.Wins)
            .ThenByDescending(p => p.GamesPlayed)
            .ToList();
    }

    public IActionResult OnPostViewGame(string gameId)
    {
        return RedirectToPage("/LoadGame");
    }

    private void CalculateStatistics()
    {
        int totalMoves = 0;
        int gameCount = 0;

        foreach (var game in AllGames)
        {
            int moveCount = BLL.GameAnalyzer.CountMovesFromState(game);
            bool isCompleted = BLL.GameAnalyzer.IsGameCompleted(game);

            if (isCompleted)
            {
                CompletedGames++;
            }
            else
            {
                InProgressGames++;
            }

            if (moveCount > 0)
            {
                totalMoves += moveCount;
                gameCount++;
            }
        }

        AverageMovesPerGame = gameCount > 0 ? (double)totalMoves / gameCount : 0;
    }

    public bool IsGameCompleted(GameState game)
    {
        return BLL.GameAnalyzer.IsGameCompleted(game);
    }

    public class PlayerStat
    {
        public string PlayerName { get; set; } = "";
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public bool IsAi { get; set; }
        public string? AiDifficulty { get; set; }
    }
}
