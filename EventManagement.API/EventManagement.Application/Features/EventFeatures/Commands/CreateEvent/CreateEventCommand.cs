using System;
using EventManagement.Application.Helpers;
using EventManagement.Application.Models.Enums;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Commands.CreateEvent
{
    public class CreateEventCommand : IRequest<Response<int>>
    {
        public string EventName { get;  set; }
        public string EventDescription { get;  set; }
        public string StartDate { get;  set; }
        public string EndDate { get;  set; }
        public PlaceType PlaceType { get;  set; }
        public bool RecurringEvent { get;  set; }
        public string City { get;  set; }
        public string Street { get;  set; }
        public string NumberStreet { get;  set; }
        public string PostalCode { get;  set; }
        public EventCategory CategoryType { get;  set; }
        public EventType EventType { get;  set; }
        [JsonConstructor]
        public CreateEventCommand(string eventName, string eventDescription, string startDate, string endDate,
            PlaceType placeType, bool recurringEvent, string city, string street, string numberStreet,
            string postalCode, EventCategory categoryType, EventType eventType)
        {
            this.EventName = eventName;
            this.EventDescription = eventDescription;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.PlaceType = placeType;
            this.RecurringEvent = recurringEvent;
            this.City = city;
            this.Street = street;
            this.NumberStreet = numberStreet;
            this.PostalCode = postalCode;
            this.CategoryType = categoryType;
            this.EventType = eventType;
        }
    }
}