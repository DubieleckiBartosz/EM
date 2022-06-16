namespace EventManagement.Application.Models.Dao
{
    public class EventOpinionDao
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Comment { get; set; }
        public int Stars { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
    }
}