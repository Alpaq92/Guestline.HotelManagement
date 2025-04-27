namespace MyApp.Models
{
    public class Booking
    {
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public required string HotelId { get; set; }
        public required string RoomRate { get; set; }
        public required string RoomType { get; set; }
    }
}
