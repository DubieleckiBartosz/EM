using System.Collections.Generic;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;

namespace EventManagement.Application.Models.Dao.EventDAOs
{
    public class EventWithApplicationDetailsDao : EventBaseDao
    {
        public List<EventApplicationDetailsDao> ApplicationDetailsList { get; set; }
    }
}