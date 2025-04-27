namespace MyApp.Models
{
    public class RoomType
    {
        public required string Code { get; set; }
        public required string Description { get; set; }
        public required IEnumerable<string> Amenities { get; set; }
        public required IEnumerable<string> Features { get; set; }
    }
}
