using System.Collections.Generic;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;

namespace EventManagement.Application.Models.Dao.EventDAOs
{
    public class EventWithApplicationsDao : EventBaseDao
    {
        public List<EventApplicationDao> EventApplications { get; set; }
    }
}
