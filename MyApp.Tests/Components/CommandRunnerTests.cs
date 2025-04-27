using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Components;
using MyApp.Interfaces;
using MyApp.Models;

namespace MyApp.Tests.Components
{
    [TestFixture]
    public class CommandRunnerTests
    {
        private readonly Mock<IArgumentParser> _argumentParser = new();
        private readonly Mock<IConsoleHandler> _consoleHandler = new();
        private readonly IList<Mock<ICommand>> _commands = [];

        private CommandRunner _unit;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            for (var i = 0; i < 3; i++)
            {
                _commands.Add(new Mock<ICommand>());
            }

            var data = new HotelData
            {
                Hotels =
                [
                    new Hotel
                    {
                        Id = "H1",
                        Name = "Hotel California",
                        Rooms =
                        [
                            new Room
                            {
                                RoomId = "1",
                                RoomType = "SGL"
                            }
                        ],
                        RoomTypes = []
                    }
                ],
                Bookings =
                [
                    new Booking
                    {
                        HotelId = "H1",
                        RoomType = "SGL",
                        RoomRate = "Prepaid",
                        Arrival = DateTime.Today,
                        Departure = DateTime.Today
                    }
                ]
            };
            _argumentParser.Setup(c => c.ParseStartupArguments(It.IsAny<IEnumerable<string>>())).Returns(data);

            _unit = new CommandRunner(_argumentParser.Object, _consoleHandler.Object, _commands.Select(c => c.Object), Mock.Of<ILogger<CommandRunner>>());
        }

        [SetUp]
        public void Setup()
        {
            _consoleHandler.Reset();

            _commands.ElementAt(0).Reset();
            _commands.ElementAt(0).Setup(c => c.CanExecute(It.Is<string>(s => string.IsNullOrWhiteSpace(s)))).Returns(false);

            _commands.ElementAt(1).Reset();
            _commands.ElementAt(1).Setup(c => c.CanExecute(It.Is<string>(s => $"{s}".StartsWith("Availability", StringComparison.InvariantCultureIgnoreCase)))).Returns(false);

            _commands.ElementAt(2).Reset();
            _commands.ElementAt(2).Setup(c => c.CanExecute(It.Is<string>(s => $"{s}".StartsWith("Search", StringComparison.InvariantCultureIgnoreCase)))).Returns(false);
        }

        [TestCase("", 0)]
        [TestCase("Availability(H1, 20240901, SGL)", 1)]
        [TestCase("Search(H1, 365, SGL)", 2)]
        public void Given_commandLine_WHEN_runner_is_called_THEN_correct_command_is_executed(string line, int index)
        {
            _commands.ElementAt(index).Setup(c => c.CanExecute(It.IsAny<string>())).Returns(true);
            _consoleHandler.Setup(c => c.ReadLine()).Returns(line);

            _unit.Run([]);

            _commands.ElementAt(0).Verify(p => p.CanExecute(line), Times.Exactly(index >= 0 ? 1 : 0));
            _commands.ElementAt(1).Verify(p => p.CanExecute(line), Times.Exactly(index >= 1 ? 1 : 0));
            _commands.ElementAt(2).Verify(p => p.CanExecute(line), Times.Exactly(index >= 2 ? 1 : 0));

            _commands.ElementAt(0).Verify(p => p.Execute(line, It.IsAny<HotelData>()), Times.Exactly(index == 0 ? 1 : 0));
            _commands.ElementAt(1).Verify(p => p.Execute(line, It.IsAny<HotelData>()), Times.Exactly(index == 1 ? 1 : 0));
            _commands.ElementAt(2).Verify(p => p.Execute(line, It.IsAny<HotelData>()), Times.Exactly(index == 2 ? 1 : 0));
        }
    }
}
