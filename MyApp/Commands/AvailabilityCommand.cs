using Microsoft.Extensions.Logging;
using MyApp.Interfaces;
using MyApp.Models;

namespace MyApp.Commands
{
    public class AvailabilityCommand : ICommand
    {
        private readonly IArgumentParser _argumentsParser;
        private readonly ILogger<AvailabilityCommand> _logger;

        public AvailabilityCommand(IArgumentParser argumentsParser, ILogger<AvailabilityCommand> logger)
        {
            _argumentsParser = argumentsParser;
            _logger = logger;
        }

        public bool CanExecute(string command)
        {
            return command?.StartsWith("Availability", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool Execute(string command, HotelData data)
        {
            var arguments = _argumentsParser.ParseCommandArguments(command);

            if(arguments != null && arguments.Count() == 3)
            {
                var hotelId = arguments.ElementAt(0) as string;
                var dateRange = arguments.ElementAt(1) as IEnumerable<DateTime>;
                var roomType = arguments.ElementAt(2) as string;

                if (!string.IsNullOrWhiteSpace(hotelId) && dateRange != null && !string.IsNullOrWhiteSpace(roomType))
                {
                    var allRoomCount = data.Hotels.FirstOrDefault(h => string.Equals(hotelId, h.Id, StringComparison.InvariantCultureIgnoreCase))
                    ?.Rooms?.Count(r => string.Equals(roomType, r.RoomType, StringComparison.InvariantCultureIgnoreCase)) ?? 0;
                    var relevantBookingsCount = data.Bookings.Count(b => string.Equals(roomType, b.RoomType, StringComparison.InvariantCultureIgnoreCase)
                        && ((b.Arrival >= dateRange.First() && b.Arrival <= dateRange.Last()) || (b.Departure >= dateRange.First() && b.Arrival <= dateRange.Last())));
                    var availableCount = allRoomCount - relevantBookingsCount;

                    _logger.LogInformation("{totalCount}", availableCount);
                }
                else
                {
                    _logger.LogWarning("Provided arguments are malformed");
                }                
            }
            else
            {
                _logger.LogWarning("Provided incorrect amount of arguments");
            }

            return true;
        }
    }
}
