using System;
using EventManagement.Application.Models.Enums;

namespace EventManagement.Application.Models.Dao.EventDAOs
{
    public class EventBaseDao
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PlaceType PlaceType { get; set; }
        public bool RecurringEvent { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string NumberStreet { get; set; }
        public string PostalCode { get; set; }
        public string CategoryName => EventCategory.ToString();
        public EventCategory EventCategory { get; set; }
        public EventType EventType { get; set; }
        public EventCurrentStatus CurrentStatus { get; set; }
    }
}
