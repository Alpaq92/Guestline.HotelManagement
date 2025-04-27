namespace MyApp.Interfaces
{
    internal interface ICommandRunner
    {
        void Run(IEnumerable<string> args);
    }
}
