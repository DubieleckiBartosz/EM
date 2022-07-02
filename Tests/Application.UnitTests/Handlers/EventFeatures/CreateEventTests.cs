using System;
using System.Threading.Tasks;
using EventManagement.Application.Features.EventFeatures.Commands.CreateEvent;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using AutoFixture;
using EventManagement.Domain.Base.EnumerationClasses;
using Xunit;

namespace Application.UnitTests.Handlers.EventFeatures
{
    public class CreateEventTests : HandlerBaseTests<CreateEventCommandHandler, CreateEventCommand, Response<int>>
    {
        public CreateEventTests()
        {
            
        }

        [Fact]
        public async Task Test()
        {

        }

        private Event GetEvent(bool isActive = true)
        {
            var recurringEvent = this._fixture.Create<bool>();
            var city = "City_Test";
            var street = "Street_Test";
            var numberStreet = "123a";
            var postalCode = "0" + this.GetRandomInt() + "-" + this.GetRandomInt(100, 1000);
            var eventName = "Event_Test";
            var eventDescription = "Event_Description_Test";
            var startDate = DateTime.Now.AddDays(this.GetRandomInt(20, 50));
            var endTime = startDate.AddDays(1);
            var address = Address.Create(city, street, numberStreet, postalCode);
            var time = EventTime.Create(startDate, endTime);
            var name = EventName.Create(eventName);
            var description = EventDescription.Create(eventDescription);
            var place =
                Enumeration.GetById<PlaceType>((int)EventManagement.Application.Models.Enums.PlaceType.Outdoors);
            var category =
                Enumeration.GetById<EventCategory>(
                    (int)EventManagement.Application.Models.Enums.EventCategory.Massive);
            var eventType =
                Enumeration.GetById<EventType>((int)EventManagement.Application.Models.Enums.EventType.Modern);
            var newEvent = Event.Create(name, description, time, place, recurringEvent, address, category,
                eventType);

            if (!isActive)
            {
                newEvent.ChangeVisibility();
            }

            return newEvent;
        }
    }
}
