using System;
using EventManagement.Application.Features.Search;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventsBySearch
{
    public class SearchEventsQuery : BaseSearchQuery, IRequest<ResponseList<EventBaseDto>>
    {
        public int? EventId { get; set; }
        public string EventName { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string City { get; set; }
        public int? PlaceType { get; set; }
        public bool? RecurringEvent { get; set; }
        public int? Category { get; set; }
        public int? EventType { get; set; }
        public int? EventCurrentStatus { get; set; }
        public SortModelQuery SortModel { get; set; }

        [JsonConstructor]
        public SearchEventsQuery(int? eventId = null, string eventName = null,
            DateTime? from = null, DateTime? to = null, string city = null, int? placeType = null,
            bool? recurringEvent = null, int? category = null, 
            int? eventType = null, int? eventCurrentStatus = null,
            SortModelQuery sortModel = null)
        {
            this.EventId = eventId;
            this.EventName = eventName;
            this.From = from;
            this.To = to;
            this.City = city;
            this.PlaceType = placeType;
            this.RecurringEvent = recurringEvent;
            this.Category = category;
            this.EventType = eventType;
            this.EventCurrentStatus = eventCurrentStatus;
            this.SortModel = sortModel;
        }
    }
}