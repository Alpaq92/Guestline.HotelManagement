using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Commands;
using MyApp.Components;
using MyApp.Models;

namespace MyApp.Tests.Commands
{
    [TestFixture]
    public class SearchCommandTests
    {
        private readonly Mock<ILogger<SearchCommand>> _logger = new();

        private SearchCommand _unit;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _unit = new SearchCommand(new ArgumentParser(Mock.Of<ILogger<ArgumentParser>>()), _logger.Object);
        }

        [TestCase("Availability(H1, 20240901, SGL)", false)]
        [TestCase("Search(H1, 365, SGL)", true)]
        [TestCase("", false)]
        public void Given_commandLine_WHEN_searchCommand_is_checked_THEN_only_for_correct_command(string line, bool expected)
        {
            Assert.That(_unit.CanExecute(line), Is.EqualTo(expected));
        }

        [Test]
        public void Given_inputData_WHEN_searchCommand_is_executed_THEN_correct_outcomes_will_be_displayed()
        {
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
                    },
                    new Hotel
                    {
                        Id = "H2",
                        Name = "Hilbert's Hotel",
                        Rooms =
                        [
                            new Room
                            {
                                RoomId = "1",
                                RoomType = "SGL"
                            },
                            new Room
                            {
                                RoomId = "2",
                                RoomType = "SGL"
                            },
                            new Room
                            {
                                RoomId = "3",
                                RoomType = "DBL"
                            },
                            new Room
                            {
                                RoomId = "4",
                                RoomType = "DBL"
                            },
                            new Room
                            {
                                RoomId = "5",
                                RoomType = "DBL"
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
                    },
                    new Booking
                    {
                        HotelId = "H1",
                        RoomType = "SGL",
                        RoomRate = "Standard",
                        Arrival = DateTime.Today,
                        Departure = DateTime.Today.AddDays(10)
                    }
                ]
            };

            var expectedH1 = $"({DateTime.Today.AddDays(11):yyyMMdd}-{DateTime.Today.AddDays(29):yyyMMdd},1)";
            Assert.That(_unit.Execute($"Search(H1, 30, SGL)", data), Is.True);
            _logger.Verify(logger =>
                logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains(expectedH1)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                )
            );

            var expectedH2 = $"({DateTime.Today:yyyMMdd}-{DateTime.Today.AddDays(9):yyyMMdd},3)";
            Assert.That(_unit.Execute($"Availability(H2, 10, DBL)", data), Is.True);
            _logger.Verify(logger =>
                logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains(expectedH2)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                )
            );
        }
    }
}
