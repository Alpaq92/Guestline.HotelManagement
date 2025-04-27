using CommandLine;

namespace MyApp.Models
{
    public class Options
    {
        [Option("hotels", Required = true, HelpText = "Load hotel data from JSON file")]
        public required string Hotels { get; set; }
        [Option("bookings", Required = true, HelpText = "Load booking data from JSON file")]
        public required string Bookings { get; set; }
    }
}
