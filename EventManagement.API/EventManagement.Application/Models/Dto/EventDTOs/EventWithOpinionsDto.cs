using System.Collections.Generic;
using EventManagement.Application.Models.Dao;

namespace EventManagement.Application.Models.Dto.EventDTOs
{
    public class EventWithOpinionsDto : EventBaseDto
    {
        public List<EventOpinionDao> Opinions { get; set; }
    }
}
