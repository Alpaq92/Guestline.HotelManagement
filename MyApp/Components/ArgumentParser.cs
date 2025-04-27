using CommandLine;
using Microsoft.Extensions.Logging;
using MyApp.Interfaces;
using MyApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

namespace MyApp.Components
{
    public partial class ArgumentParser : IArgumentParser
    {
        private readonly ILogger<ArgumentParser> _logger;

        public ArgumentParser(ILogger<ArgumentParser> logger)
        {
            _logger = logger;
        }


        public HotelData ParseStartupArguments(IEnumerable<string> args)
        {
            var result = new HotelData();

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(o =>
                   {
                       result.Hotels = LoadData<IEnumerable<Hotel>>(o.Hotels) ?? [];
                       result.Bookings = LoadData<IEnumerable<Booking>>(o.Bookings) ?? [];
                   });

            return result;
        }

        public IEnumerable<object> ParseCommandArguments(string line)
        {
            var result = new List<object>();
            var args = Arguments().Matches(line)?.Select(v => v.Value)?.OrderByDescending(s => s.Length)?.FirstOrDefault()
                ?.Replace("(", "")?.Replace(")", "").Split(",");

            if (args != null)
            {
                foreach (var argument in args)
                {
                    var item = argument.Trim();

                    if (DateRange().IsMatch(item))
                    {
                        var splitted = item.Split("-");

                        if (splitted.Length == 2)
                        {
                            var parsedStart = DateTime.TryParseExact(splitted[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var start);
                            var parsedEnd = DateTime.TryParseExact(splitted[1], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var end);

                            if (parsedStart && parsedEnd)
                            {
                                result.Add(new[] { start, end });
                            }
                            else
                            {
                                _logger.LogWarning("Unable to parse date range argument");
                            }
                        }
                    }
                    else if (SingleDate().IsMatch(item))
                    {
                        var parsed = DateTime.TryParseExact(item, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date);
                        
                        if (parsed)
                        {
                            result.Add(new[] { date, date });
                        }
                        else
                        {
                            _logger.LogWarning("Unable to parse single date argument");
                        }
                    }
                    else
                    {
                        result.Add(item.Trim());
                    }
                }
            }
            else
            {
                _logger.LogWarning("Provided arguments are malformed");
            }

            return result;
        }

        private static T? LoadData<T>(string path)
        {
            if (File.Exists(path))
            {
                using (var stream = new StreamReader(path))
                {
                    return JsonConvert.DeserializeObject<T>(stream.ReadToEnd(), new IsoDateTimeConverter { DateTimeFormat = "yyyyMMdd" });
                }
            }

            return default;
        }

        [GeneratedRegex(@"\(.+\)", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex Arguments();
        [GeneratedRegex(@"[0-9]{8}-[0-9]{8}", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex DateRange();
        [GeneratedRegex(@"[0-9]{8}", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex SingleDate();
    }
}