using System.Collections.Generic;

namespace EventManagement.Application.Models.Dao.EventDAOs
{
    public class EventWithImagesDao : EventBaseDao
    {
        public List<EventImageDao> Images { get; set; }
    }
}