using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Features.EventFeatures.Commands.CreateEvent;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using AutoFixture;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Domain.Base.EnumerationClasses;
using Moq;
using Xunit;

namespace Application.UnitTests.Handlers.EventFeatures
{
    public class CreateEventTests : HandlerBaseTests<CreateEventCommandHandler, CreateEventCommand, Response<int>>
    {
        private readonly Mock<IBackgroundService> _backgroundServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;

        public CreateEventTests()
        {
            this._backgroundServiceMock = this._mocker.GetMock<IBackgroundService>();
            this._cacheServiceMock = this._mocker.GetMock<ICacheService>();
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Request_Is_null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.Handle((CreateEventCommand) null, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Should_Create_Event()
        {
            //Arrange
            var request = this.CreateRequest();
            var eventId = this._fixture.Create<int>();

            var eventRepository = this._mocker.GetMock<IEventRepository>();

            eventRepository.Setup(_ => _.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(eventId);

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(eventRepository.Object);

            //Act
            var response = await this._handler.Handle(request, It.IsAny<CancellationToken>());
           
            //Assert
            Assert.True(response.Success);
            this._unitOfWork.Verify(v => v.CompleteAsync(It.IsAny<Event>()), Times.Once);
        }

        private CreateEventCommand CreateRequest()
        {
            var recurringEvent = this._fixture.Create<bool>();
            var city = "City_Test";
            var street = "Street_Test";
            var numberStreet = "123a";
            var postalCode = "0" + this.GetRandomInt() + "-" + this.GetRandomInt(100, 1000);
            var eventName = "Event_Test";
            var eventDescription = "Event_Description_Test";
            var startDate = DateTime.Now.AddDays(this.GetRandomInt(20, 50));
            var endTime = startDate.AddDays(1).ToLongDateString();
            var placeType = EventManagement.Application.Models.Enums.PlaceType.Outdoors;
            var eventCategory = EventManagement.Application.Models.Enums.EventCategory.Massive;
            var eventType = EventManagement.Application.Models.Enums.EventType.Modern;
            var request = new CreateEventCommand(eventName, eventDescription, startDate.ToLongDateString(), endTime,
                placeType, recurringEvent, city, street, numberStreet, postalCode, eventCategory, eventType);

            return request;
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
                Enumeration.GetById<PlaceType>((int) EventManagement.Application.Models.Enums.PlaceType.Outdoors);
            var category =
                Enumeration.GetById<EventCategory>(
                    (int) EventManagement.Application.Models.Enums.EventCategory.Massive);
            var eventType =
                Enumeration.GetById<EventType>((int) EventManagement.Application.Models.Enums.EventType.Modern);
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