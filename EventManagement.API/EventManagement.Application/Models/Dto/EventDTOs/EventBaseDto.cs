using System;
using EventManagement.Application.Models.Enums;

namespace EventManagement.Application.Models.Dto.EventDTOs
{
    public class EventBaseDto
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool RecurringEvent { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string NumberStreet { get; set; }
        public string PostalCode { get; set; }
        public string CategoryName { get; set; }
        public string EventType { get; set; }
        public string EventCategory { get; set; } 
        public string PlaceType { get; set; }
        public string CurrentStatus { get; set; }

    }
}
