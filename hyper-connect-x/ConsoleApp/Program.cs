using System.Text;
using ConsoleApp.Menus;

namespace ConsoleApp
{
    public abstract class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var menuManager = new MenuManager();
            menuManager.Start();
        }
    }
}