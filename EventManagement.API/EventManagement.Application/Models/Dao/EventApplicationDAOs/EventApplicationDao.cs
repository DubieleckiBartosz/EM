using EventManagement.Application.Models.Enums;

namespace EventManagement.Application.Models.Dao.EventApplicationDAOs
{
    public class EventApplicationDao
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int PerformerId { get; set; }
        public TypePerformance TypePerformance { get; set; }
        public int DurationInMinutes { get; set; }
        public StatusApplication CurrentStatus { get; set; }
        public bool LastModifiedByApplicant { get; set; }
    }
}
