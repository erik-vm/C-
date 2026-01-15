namespace ConsoleApp.Menus;

public abstract class BaseMenu(string title, EMenuType menuType)
{
    protected string Title { get; } = title;
    public EMenuType MenuType { get; } = menuType;
    protected List<MenuOption> Options { get; } = [];
    public Action? OnBack { get; set; }
    protected int SelectedIndex = 0;

    protected virtual void Display()
    {
        Console.Clear();
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine(Title);
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();

        for (int i = 0; i < Options.Count; i++)
        {
            if (i == SelectedIndex)
            {
                // Highlight selected option
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"> [{Options[i].Shortcut}] {Options[i].DisplayText}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  [{Options[i].Shortcut}] {Options[i].DisplayText}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Use ↑↓ arrows to navigate, Enter to select, or press shortcut key");
    }

    public void Run()
    {
        SelectedIndex = 0;
        while (true)
        {
            Display();
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    SelectedIndex = (SelectedIndex - 1 + Options.Count) % Options.Count;
                    break;

                case ConsoleKey.DownArrow:
                    SelectedIndex = (SelectedIndex + 1) % Options.Count;
                    break;

                case ConsoleKey.Enter:
                    Console.Clear();
                    Options[SelectedIndex].OnSelect?.Invoke();
                    break;

                default:
                    var selectedOption = Options.FirstOrDefault(o =>
                        char.ToUpper(o.Shortcut) == char.ToUpper(key.KeyChar));

                    if (selectedOption != null)
                    {
                        Console.Clear();
                        selectedOption.OnSelect?.Invoke();
                    }

                    break;
            }
        }
    }

    protected abstract void InitializeOptions();


    protected void Exit()
    {
        Console.Clear();
        Console.WriteLine("Thanks for playing! Goodbye!");
        Environment.Exit(0);
    }

    protected void Back()
    {
        OnBack?.Invoke();
    }
}