using MyApp.Interfaces;

namespace MyApp.Helpers
{
    public class ConsoleHandler : IConsoleHandler
    {
        public string? ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
