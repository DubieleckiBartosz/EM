using System.Collections.Generic;

namespace EventManagement.Application.Models.Dto.EventDTOs
{
    public class EventDetailsDto : EventBaseDto
    {
        public List<EventImageDto> Images { get; set; }
        public List<EventOpinionDto> Opinions { get; set; }
    }
}
