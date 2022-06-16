using EventManagement.Application.Models.Enums;

namespace EventManagement.Application.Models.Dto
{
    public class EventApplicationDto
    {
        public int Id { get; set; }

        public int PerformerId { get; set; }

        //  public TypePerformance TypePerformance { get; set; }
        public int DurationInMinutes { get; set; }

        // public StatusApplication CurrentStatus { get; set; }
        public bool LastModifiedByApplicant { get; set; }
        public string TypePerformance { get; set; }
        public string CurrentStatus { get; set; }
    }
}