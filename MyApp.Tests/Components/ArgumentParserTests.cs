using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Components;

namespace MyApp.Tests.Components
{
    [TestFixture]
    public class ArgumentParserTests
    {
        private ArgumentParser _unit;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _unit = new ArgumentParser(Mock.Of<ILogger<ArgumentParser>>());
        }

        [Test]
        public void Given_starupArguments_WHEN_parsed_THEN_data_is_correctly_loaded()
        {
            var result = _unit.ParseStartupArguments(["MyApp", "--hotels", "Docs/hotels.json", "--bookings", "Docs/bookings.json"]);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Hotels, Is.Not.Null);
            Assert.That(result.Bookings, Is.Not.Null);
        }

        [Test]
        public void Given_missing_starupArguments_WHEN_parsed_THEN_null_is_returned()
        {
            var result = _unit.ParseStartupArguments(["MyApp"]);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Hotels, Is.Empty);
            Assert.That(result.Bookings, Is.Empty);
        }

        [TestCase("Availability(H1, 20240901, SGL)")]
        [TestCase("Availability(H1, 20240901-20240905, SGL)")]
        [TestCase("Search(H1, 365, SGL)")]
        public void Given_commandLine_WHEN_parsed_THEN_correctly_formatted(string line)
        {
            var result = _unit.ParseCommandArguments(line);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Given_commandLine_with_date_WHEN_parsed_THEN_transformed_to_range()
        {
            var result = _unit.ParseCommandArguments("Availability(H1, 20240901, SGL)");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));

            var range = result.ElementAt(1) as IEnumerable<DateTime>;

            Assert.That(range, Is.Not.Null);
            Assert.That(range.FirstOrDefault().ToString("yyyyMMdd"), Is.EqualTo("20240901"));
            Assert.That(range.LastOrDefault().ToString("yyyyMMdd"), Is.EqualTo("20240901"));

            result = _unit.ParseCommandArguments("Availability(H1, 20240901-20240905, SGL)");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));

            range = result.ElementAt(1) as IEnumerable<DateTime>;

            Assert.That(range, Is.Not.Null);
            Assert.That(range.FirstOrDefault().ToString("yyyyMMdd"), Is.EqualTo("20240901"));
            Assert.That(range.LastOrDefault().ToString("yyyyMMdd"), Is.EqualTo("20240905"));
        }
    }
}
