namespace MyApp.Models
{
    public class SearchResult : ICloneable
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RoomCount { get; set; }

        public override string ToString()
        {
            return $"({StartDate:yyyyMMdd}-{EndDate:yyyyMMdd},{RoomCount})";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
