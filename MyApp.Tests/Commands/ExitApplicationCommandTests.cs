using MyApp.Commands;
using MyApp.Models;

namespace MyApp.Tests.Commands
{
    [TestFixture]
    public class ExitApplicationCommandTests
    {
        [TestCase("", true)]
        [TestCase("   ", true)]
        [TestCase("test", false)]
        [TestCase("Availability(H1, 20240901, SGL)", false)]
        [TestCase("Search(H1, 365, SGL)", false)]
        public void Given_commandLine_WHEN_exitApplicationCommand_is_checked_THEN_only_for_correct_command(string line, bool expected)
        {
            Assert.That(new ExitApplicationCommand().CanExecute(line), Is.EqualTo(expected));
        }

        [Test]
        public void Given_exitApplicationCommand_WHEN_command_is_executed_THEN_result_is_false()
        {
            Assert.That(new ExitApplicationCommand().Execute(string.Empty, new HotelData()), Is.False);
        }
    }
}
