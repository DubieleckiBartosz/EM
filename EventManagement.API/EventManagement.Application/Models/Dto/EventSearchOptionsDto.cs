using System.Collections.Generic;
using EventManagement.Application.Helpers;
using EventManagement.Application.Models.Enums;

namespace EventManagement.Application.Models.Dto
{
    public class EventSearchOptionsDto
    {
        public List<string> Categories { get; }
        public List<string> PlaceTypes { get; }
        public List<string> EventTypes { get; }
        public List<string> EventStatuses { get; }

        public EventSearchOptionsDto()
        {
            this.Categories = EnumHelpers.GetStringValuesFromEnum<EventCategory>();
            this.PlaceTypes = EnumHelpers.GetStringValuesFromEnum<PlaceType>();
            this.EventTypes = EnumHelpers.GetStringValuesFromEnum<EventType>();
            this.EventStatuses = EnumHelpers.GetStringValuesFromEnum<EventCurrentStatus>();
        }
    }
}