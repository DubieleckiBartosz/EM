using System.Collections.Generic;

namespace EventManagement.Application.Models.Dao.EventDAOs
{
    public class EventDetailsDao : EventBaseDao
    {
        public List<EventImageDao> Images { get; set; }
        public List<EventOpinionDao> Opinions { get; set; }
    }
}