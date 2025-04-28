using Microsoft.Extensions.Logging;
using MyApp.Interfaces;

namespace MyApp.Components
{
    public class CommandRunner : ICommandRunner
    {
        private readonly IArgumentParser _argumentsParser;
        private readonly IConsoleHandler _consoleHandler;
        private readonly IEnumerable<ICommand> _commands;
        private readonly ILogger<CommandRunner> _logger;

        public CommandRunner(IArgumentParser argumentsParser, IConsoleHandler consoleHandler, IEnumerable<ICommand> commands, ILogger<CommandRunner> logger)
        {
            _argumentsParser = argumentsParser;
            _consoleHandler = consoleHandler;
            _commands = commands;
            _logger = logger;
        }

        public void Run(IEnumerable<string> args)
        {
            var data = _argumentsParser.ParseStartupArguments(args);

            if (data != null && (data.Hotels?.Any() ?? false) && (data.Bookings?.Any() ?? false))
            {
                _logger.LogInformation("Successfully started Guestline Hotel Management");

                var shouldContinue = true;
                while (shouldContinue)
                {
                    var input = _consoleHandler.ReadLine() ?? string.Empty;

                    var command = _commands.FirstOrDefault(x => x.CanExecute(input));
                    if (command != null)
                    {
                        _logger.LogInformation($"Found command {command.GetType().Name}");
                        shouldContinue = command.Execute(input, data);
                    }
                    else
                    {
                        _logger.LogInformation("Could not suited command");
                    }
                }
            }
        }
    }
}
