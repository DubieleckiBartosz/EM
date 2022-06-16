using EventManagement.Application.Models.Enums;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Commands.UpdateEvent
{
    public class UpdateEventCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }
        public string EventDescription { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public PlaceType? PlaceType { get; set; }
        public bool? RecurringEvent { get; set; }
        public EventCategory? CategoryType { get; set; }
        public EventType? EventType { get; set; }

        [JsonConstructor]
        public UpdateEventCommand(int eventId, string eventDescription = null, string startDate = null,
            string endDate = null,
            PlaceType? placeType = null, bool? recurringEvent = null, EventCategory? categoryType = null,
            EventType? eventType = null)
        {
            this.EventId = eventId;
            this.EventDescription = eventDescription;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.PlaceType = placeType;
            this.RecurringEvent = recurringEvent;
            this.CategoryType = categoryType;
            this.EventType = eventType;
        }
    }
}