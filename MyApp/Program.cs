using Autofac;
using Microsoft.Extensions.Logging;
using MyApp.Commands;
using MyApp.Components;
using MyApp.Helpers;
using MyApp.Interfaces;

var container = BuildContainer();
var commandRunner = container.Resolve<ICommandRunner>();

commandRunner.Run(Environment.GetCommandLineArgs());

static IContainer BuildContainer()
{
    var builder = new ContainerBuilder();

    builder.Register(handler => LoggerFactory.Create(ConfigureLogging))
        .As<ILoggerFactory>()
        .SingleInstance()
        .AutoActivate();

    builder.RegisterGeneric(typeof(Logger<>))
        .As(typeof(ILogger<>))
        .SingleInstance();

    builder.RegisterType<ConsoleHandler>()
           .As<IConsoleHandler>()
           .SingleInstance();

    builder.RegisterType<CommandRunner>()
           .As<ICommandRunner>()
           .SingleInstance();

    builder.RegisterType<ArgumentParser>()
           .As<IArgumentParser>()
           .SingleInstance();

    builder.RegisterType<ExitApplicationCommand>()
           .As<ICommand>()
           .SingleInstance();

    builder.RegisterType<AvailabilityCommand>()
           .As<ICommand>()
           .SingleInstance();

    builder.RegisterType<SearchCommand>()
           .As<ICommand>()
           .SingleInstance();

    return builder.Build();
}

static void ConfigureLogging(ILoggingBuilder log)
{
    log.ClearProviders();
    log.SetMinimumLevel(LogLevel.Information);
    log.AddConsole();
}
