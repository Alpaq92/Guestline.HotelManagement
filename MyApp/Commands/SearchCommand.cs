using Microsoft.Extensions.Logging;
using MyApp.Interfaces;
using MyApp.Models;

namespace MyApp.Commands
{
    public class SearchCommand : ICommand
    {
        private readonly IArgumentParser _argumentsParser;
        private readonly ILogger<SearchCommand> _logger;

        public SearchCommand(IArgumentParser argumentsParser, ILogger<SearchCommand> logger)
        {
            _argumentsParser = argumentsParser;
            _logger = logger;
        }

        public bool CanExecute(string command)
        {
            return command?.StartsWith("Search", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool Execute(string command, HotelData data)
        {
            var results = new List<SearchResult>();
            var arguments = _argumentsParser.ParseCommandArguments(command);

            if (arguments != null && arguments.Count() == 3)
            {
                var hotelId = arguments.ElementAt(0) as string;
                var rangeSize = int.TryParse(arguments.ElementAt(1)?.ToString(), out var value) ? value : 0;
                var roomType = arguments.ElementAt(2) as string;

                var allRooms = data.Hotels.FirstOrDefault(h => string.Equals(hotelId, h.Id, StringComparison.InvariantCultureIgnoreCase))
                   ?.Rooms?.Where(r => string.Equals(roomType, r.RoomType, StringComparison.InvariantCultureIgnoreCase));
                var availabilities = Enumerable.Repeat(allRooms?.Count() ?? 0, rangeSize).Select((value, index) => new { index, value })
                    .ToDictionary(pair => DateTime.Today.AddDays(pair.index), pair => pair.value);

                if (availabilities?.Count > 0)
                {
                    var relevantBookings = data.Bookings.Where(b => string.Equals(roomType, b.RoomType, StringComparison.InvariantCultureIgnoreCase));
                    foreach (var booking in relevantBookings)
                    {
                        var dates = Enumerable.Range(0, 1 + booking.Departure.Subtract(booking.Arrival).Days).Select(offset => booking.Arrival.AddDays(offset));
                        foreach (var date in dates)
                        {
                            if (availabilities.ContainsKey(date))
                            {
                                availabilities[date] -= 1;
                            }
                        }
                    }
                    
                    var first = availabilities.FirstOrDefault();
                    var item = new SearchResult { StartDate = first.Key, EndDate = first.Key, RoomCount = first.Value };

                    for (var index = 0; index < availabilities.Count; index++)
                    {
                        var availibility = availabilities.ElementAtOrDefault(index);

                        if (item.RoomCount == availibility.Value)
                        {
                            item.EndDate = availibility.Key;
                        }
                        else
                        {
                            if (item.RoomCount > 0)
                            {
                                _logger.LogInformation("{item}", item);
                                results.Add((SearchResult)item.Clone());
                            }

                            item.StartDate = availibility.Key;
                            item.EndDate = availibility.Key;
                            item.RoomCount = availibility.Value;
                        }

                        if (index == availabilities.Count - 1 && item.RoomCount > 0)
                        {
                            _logger.LogInformation("{item}", item);
                            results.Add(item);
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("Provided incorrect amount of arguments");
            }

            if (results.Count == 0)
            {
                _logger.LogWarning("Did not found any fits");
            }
            

            return true;
        }
    }
}
