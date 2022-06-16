namespace EventManagement.Application.Models.Dao
{
    public class EventImageDao
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string ImagePath { get; set; }
        public string ImageTitle { get; set; }
        public bool IsMain { get; set; }
        public string ImageDescription { get; set; }
    }
}