using MyApp.Models;

namespace MyApp.Interfaces
{
    public interface ICommand
    {
        bool CanExecute(string command);
        bool Execute(string command, HotelData data);
    }
}
