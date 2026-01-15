using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Services;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IPlayerRepository _playerRepository;

    public IndexModel(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public List<PlayerProfile> HumanPlayers { get; set; } = new();
    public List<PlayerProfile> AiPlayers { get; set; } = new();

    [BindProperty]
    public string Player1Selection { get; set; } = "";

    [BindProperty]
    public string? NewPlayer1Name { get; set; }

    [BindProperty]
    public string Player2Selection { get; set; } = "";

    [BindProperty]
    public string? NewPlayer2Name { get; set; }

    [BindProperty]
    public int BoardHeight { get; set; } = GameConfiguration.DefaultBoardHeight;

    [BindProperty]
    public int BoardWidth { get; set; } = GameConfiguration.DefaultBoardWidth;

    [BindProperty]
    public int WinCondition { get; set; } = GameConfiguration.DefaultWinCondition;

    [BindProperty]
    public string BoardShape { get; set; } = "Rectangle";

    public void OnGet()
    {
        LoadPlayers();
    }

    private void LoadPlayers()
    {
        var allPlayers = _playerRepository.GetAllPlayers();
        HumanPlayers = allPlayers.Where(p => !p.IsAi).ToList();
        AiPlayers = allPlayers.Where(p => p.IsAi).ToList();
    }

    public IActionResult OnPost()
    {
        LoadPlayers();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var player1Profile = GetOrCreatePlayer(Player1Selection, NewPlayer1Name, "NewPlayer1Name");
        if (player1Profile == null)
        {
            return Page();
        }

        var player2Profile = GetOrCreatePlayer(Player2Selection, NewPlayer2Name, "NewPlayer2Name");
        if (player2Profile == null)
        {
            return Page();
        }

        var player1 = new Player(
            player1Profile.PlayerName,
            GameConfiguration.DefaultPlayer1Color,
            player1Profile.IsAi,
            player1Profile.AiDifficulty
        );

        var player2 = new Player(
            player2Profile.PlayerName,
            GameConfiguration.DefaultPlayer2Color,
            player2Profile.IsAi,
            player2Profile.AiDifficulty
        );

        var random = new Random();
        var startingPlayer = random.Next(2) == 0 ? player1 : player2;

        var game = new Game(
            player1,
            player2,
            BoardHeight,
            BoardWidth,
            BoardShape,
            WinCondition,
            startingPlayer
        );

        GameSessionService.SaveGameToSession(HttpContext.Session, game, 0);

        return RedirectToPage("/Play");
    }

    private PlayerProfile? GetOrCreatePlayer(string selection, string? newPlayerName, string modelErrorKey)
    {
        if (selection == "new")
        {
            if (string.IsNullOrWhiteSpace(newPlayerName))
            {
                ModelState.AddModelError(modelErrorKey, "Player name is required");
                return null;
            }

            var profile = new PlayerProfile
            {
                PlayerName = newPlayerName,
                IsAi = false,
                CreatedAt = DateTime.Now,
                TotalGamesPlayed = 0,
                TotalWins = 0
            };
            _playerRepository.SavePlayer(profile);
            return profile;
        }

        var existingProfile = _playerRepository.GetPlayer(selection);
        if (existingProfile == null)
        {
            ModelState.AddModelError(modelErrorKey, "Player not found");
            return null;
        }

        return existingProfile;
    }
}
