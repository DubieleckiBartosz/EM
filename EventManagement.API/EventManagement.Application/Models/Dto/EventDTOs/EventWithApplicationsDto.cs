using System.Collections.Generic;

namespace EventManagement.Application.Models.Dto.EventDTOs
{
    public class EventWithApplicationsDto : EventBaseDto
    {
        public List<EventApplicationDto> EventApplications { get; set; }
    }
}
