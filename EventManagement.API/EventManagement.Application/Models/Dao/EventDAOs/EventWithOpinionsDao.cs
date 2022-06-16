using System.Collections.Generic;

namespace EventManagement.Application.Models.Dao.EventDAOs
{
    public class EventWithOpinionsDao : EventBaseDao
    {
        public List<EventOpinionDao> Opinions { get; set; }
    }
}