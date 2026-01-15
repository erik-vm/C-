namespace ConsoleApp.Menus;

public class MenuOption(char shortcut, string displayText, Action onSelect)
{
    public char Shortcut { get; } = shortcut;
    public string DisplayText { get; } = displayText;
    public Action OnSelect { get; } = onSelect;
}