namespace MyApp.Models
{
    public class Hotel
    {
        public required string Id {  get; set; }
        public required string Name { get; set; }
        public required IEnumerable<Room> Rooms { get; set; }
        public required IEnumerable<RoomType> RoomTypes { get; set; }
    }
}
