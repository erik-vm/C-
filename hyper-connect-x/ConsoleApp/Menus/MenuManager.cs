namespace ConsoleApp.Menus;

public class MenuManager
{
    private readonly Dictionary<EMenuType, BaseMenu> _menus;
    private BaseMenu? _currentMenu;

    public MenuManager()
    {
        _menus = new Dictionary<EMenuType, BaseMenu>();
        InitializeMenus();
    }

    private void InitializeMenus()
    {
        var mainMenu = new MainMenu(this);
        var startMenu = new StartMenu(this);
        var settingsMenu = new SettingsMenu();
        var customGameMenu = new CustomGameMenu(this);

        mainMenu.OnBack = null;
        startMenu.OnBack = () => NavigateTo(EMenuType.Main);
        settingsMenu.OnBack = () => NavigateTo(EMenuType.Main);
        customGameMenu.OnBack = () => NavigateTo(EMenuType.Main);

        _menus.Add(EMenuType.Main, mainMenu);
        _menus.Add(EMenuType.Start, startMenu);
        _menus.Add(EMenuType.Settings, settingsMenu);
        _menus.Add(EMenuType.CustomGame, customGameMenu);
    }

    public void NavigateTo(EMenuType menuType)
    {
        if (_menus.TryGetValue(menuType, out var menu))
        {
            _currentMenu = menu;
            _currentMenu.Run();
        }
    }

    public void Start()
    {
        NavigateTo(EMenuType.Main);
    }
}