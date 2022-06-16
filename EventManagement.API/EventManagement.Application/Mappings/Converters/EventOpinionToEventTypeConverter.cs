﻿using System.Collections.Generic;
using AutoMapper;
using EventManagement.Application.Models.Dao.EventDAOs;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Application.Mappings.Converters
{
    public class EventOpinionToEventTypeConverter : ITypeConverter<EventWithOpinionsDao, Event>
    {
        public Event Convert(EventWithOpinionsDao source, Event destination, ResolutionContext context)
        {
            var eventName = EventName.Create(source.EventName);
            var description = EventDescription.Create(source.EventDescription);
            var time = EventTime.Create(source.StartDate, source.EndDate);
            var placeType = Enumeration.GetById<PlaceType>((int) source.PlaceType);
            var recurringEvent = source.RecurringEvent;
            var address = Address.Create(source.City, source.Street, source.NumberStreet, source.PostalCode);
            var category = Enumeration.GetById<EventCategory>((int) source.EventCategory);
            var eventType = Enumeration.GetById<EventType>((int) source.EventType);
            var opinions = context.Mapper.Map<List<EventOpinion>>(source.Opinions);
            var eventStatus = Enumeration.GetById<EventCurrentStatus>((int)source.CurrentStatus);

            return Event.LoadEvent(source.Id, null, opinions, null, eventName, description, time, placeType,
                eventStatus, recurringEvent, address, category, eventType);
        }
    }
}