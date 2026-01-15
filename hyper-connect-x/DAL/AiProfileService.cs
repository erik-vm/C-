namespace DAL;

public static class AiProfileService
{
    private const string EasyDifficulty = "Easy";
    private const string MediumDifficulty = "Medium";
    private const string HardDifficulty = "Hard";

    private static readonly List<(string Name, string Difficulty)> AiProfiles = new()
    {
        ("NOVA", EasyDifficulty),
        ("CORTEX", MediumDifficulty),
        ("ZENITH", HardDifficulty),
        ("Watson", MediumDifficulty),
        ("Deep Blue", HardDifficulty),
        ("AlphaZero", HardDifficulty),
        ("Stockfish", HardDifficulty),
        ("HAL 9000", MediumDifficulty),
        ("Skynet", HardDifficulty),
        ("ARIA", EasyDifficulty),
        ("NEXUS", MediumDifficulty),
        ("QUANTUM", HardDifficulty)
    };

    public static List<string> GetAllAiNames()
    {
        return AiProfiles.Select(p => p.Name).ToList();
    }

    public static string GetDefaultDifficulty(string aiName)
    {
        var profile = AiProfiles.FirstOrDefault(p => p.Name == aiName);
        return profile.Difficulty ?? MediumDifficulty;
    }

    public static List<PlayerProfile> GetPredefinedAiProfiles()
    {
        return AiProfiles.Select(p => CreateAiProfile(p.Name, p.Difficulty)).ToList();
    }

    private static PlayerProfile CreateAiProfile(string name, string difficulty)
    {
        return new PlayerProfile
        {
            PlayerName = name,
            IsAi = true,
            AiDifficulty = difficulty,
            CreatedAt = DateTime.Now,
            TotalGamesPlayed = 0,
            TotalWins = 0
        };
    }

    public static void InitializeAiProfiles(IPlayerRepository repository)
    {
        var existingPlayers = repository.GetAllPlayers();
        var aiProfiles = GetPredefinedAiProfiles();

        foreach (var aiProfile in aiProfiles)
        {
            if (!existingPlayers.Any(p => p.PlayerName == aiProfile.PlayerName))
            {
                repository.SavePlayer(aiProfile);
            }
        }
    }
}
