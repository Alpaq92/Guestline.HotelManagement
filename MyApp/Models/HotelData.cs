namespace MyApp.Models
{
    public class HotelData
    {
        public IEnumerable<Hotel> Hotels { get; set; } = [];
        public IEnumerable<Booking> Bookings { get; set; } = [];
    }
}