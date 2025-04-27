using MyApp.Interfaces;
using MyApp.Models;

namespace MyApp.Commands
{
    public class ExitApplicationCommand : ICommand
    {
        public bool CanExecute(string command)
        {
            return string.IsNullOrWhiteSpace(command);
        }

        public bool Execute(string command, HotelData data)
        {
            return false;
        }
    }
}
