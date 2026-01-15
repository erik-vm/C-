using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;
using WebApp.Services;

namespace WebApp.Pages;

public class PlayModel : PageModel
{
    private readonly IGameRepository _repository;
    private readonly PlayerStatisticsService _statisticsService;
    private readonly IHubContext<GameHub> _hubContext;

    public PlayModel(IGameRepository repository, PlayerStatisticsService statisticsService, IHubContext<GameHub> hubContext)
    {
        _repository = repository;
        _statisticsService = statisticsService;
        _hubContext = hubContext;
    }

    public Game? Game { get; set; }
    public int MoveCount { get; set; }
    private string? GameId { get; set; }

    public void OnGet()
    {
        LoadGameFromSession();
    }

    public async Task<IActionResult> OnPostMakeMove(int column)
    {
        LoadGameFromSession();

        if (Game == null || Game.IsGameOver)
        {
            return RedirectToPage();
        }

        if (Game.CurrentPlayer.IsAi)
        {
            TempData["Error"] = "It's the AI's turn!";
            return RedirectToPage();
        }

        bool moveSuccess = Game.MakeMove(column);

        if (!moveSuccess)
        {
            TempData["Error"] = "Invalid move!";
            return RedirectToPage();
        }

        MoveCount++;

        // Auto-save completed games for statistics
        if (Game.IsGameOver)
        {
            AutoSaveCompletedGame();
        }

        SaveGameToSession();

        var gameSessionId = GetGameSessionId();
        await _hubContext.Clients.Group(gameSessionId).SendAsync("GameUpdated");

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostProcessAiMove()
    {
        LoadGameFromSession();

        if (Game == null || Game.IsGameOver)
        {
            return RedirectToPage();
        }

        if (!Game.CurrentPlayer.IsAi)
        {
            return RedirectToPage();
        }

        int column = Game.CurrentPlayer.GetAiMove(Game);
        Game.MakeMove(column);
        MoveCount++;

        // Auto-save completed games for statistics
        if (Game.IsGameOver)
        {
            AutoSaveCompletedGame();
        }

        SaveGameToSession();

        var gameSessionId = GetGameSessionId();
        await _hubContext.Clients.Group(gameSessionId).SendAsync("GameUpdated");

        return RedirectToPage();
    }

    public IActionResult OnPostSaveGame()
    {
        LoadGameFromSession();

        if (Game == null)
        {
            return RedirectToPage();
        }

        var gameState = BLL.GameStateConverter.ToGameState(Game, GameId ?? "");
        GameId = _repository.SaveGame(gameState);

        TempData["Success"] = $"Game saved! ID: {GameId}";

        SaveGameToSession();

        return RedirectToPage();
    }

    public IActionResult OnPostPlayAgain()
    {
        LoadGameFromSession();

        if (Game == null)
        {
            return RedirectToPage("/");
        }

        var random = new Random();
        var startingPlayer = random.Next(2) == 0 ? Game.Player1 : Game.Player2;

        var newGame = new Game(
            Game.Player1,
            Game.Player2,
            Game.Board.Height,
            Game.Board.Width,
            Game.Board.Shape,
            Game.WinningCondition,
            startingPlayer
        );

        Game = newGame;
        MoveCount = 0;
        GameId = null;

        SaveGameToSession();

        return RedirectToPage();
    }

    private void LoadGameFromSession()
    {
        var (game, moveCount, gameId) = GameSessionService.LoadGameFromSession(HttpContext.Session);
        Game = game;
        MoveCount = moveCount;
        GameId = gameId;
    }

    private void SaveGameToSession()
    {
        if (Game == null)
        {
            return;
        }

        GameSessionService.SaveGameToSession(HttpContext.Session, Game, MoveCount, GameId);
    }

    private void AutoSaveCompletedGame()
    {
        if (Game == null || !Game.IsGameOver)
        {
            return;
        }

        var gameState = GameStateConverter.ToGameState(Game, GameId ?? Guid.NewGuid().ToString());
        GameId = _repository.SaveGame(gameState);

        _statisticsService.RecordGameCompletion(Game);
    }

    private string GetGameSessionId()
    {
        return HttpContext.Session.Id;
    }
}
