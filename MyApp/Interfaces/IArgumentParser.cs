using MyApp.Models;

namespace MyApp.Interfaces
{
    public interface IArgumentParser
    {
        HotelData ParseStartupArguments(IEnumerable<string> args);
        IEnumerable<object> ParseCommandArguments(string line);
    }
}
