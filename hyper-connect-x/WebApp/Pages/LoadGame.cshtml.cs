using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Services;

namespace WebApp.Pages;

public class LoadGameModel : PageModel
{
    private readonly IGameRepository _repository;

    public LoadGameModel(IGameRepository repository)
    {
        _repository = repository;
    }

    public List<GameState> SavedGames { get; set; } = new();

    public void OnGet()
    {
        SavedGames = _repository.GetAllGames();
    }

    public IActionResult OnPostLoadGame(string gameId)
    {
        var gameState = _repository.LoadGame(gameId);

        if (gameState == null)
        {
            TempData["Error"] = "Game not found!";
            return RedirectToPage();
        }

        var game = GameStateConverter.ToGame(gameState);
        int moveCount = GameAnalyzer.CountMoves(game);

        GameSessionService.SaveGameToSession(HttpContext.Session, game, moveCount, gameId);

        return RedirectToPage("/Play");
    }

    public IActionResult OnPostDeleteGame(string gameId)
    {
        _repository.DeleteGame(gameId);
        TempData["Success"] = "Game deleted successfully!";
        return RedirectToPage();
    }
}
