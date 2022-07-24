using System;
using System.Threading;
using EventManagement.Application.Features.EventFeatures.Commands.ChangeVisibilityEvent;
using EventManagement.Application.Wrappers;
using System.Threading.Tasks;
using AutoFixture;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using EventManagement.Application.Models.Dao.EventDAOs;
using EventManagement.Domain.Base;
using EventManagement.Domain.Rules;
using Moq;
using Xunit;
using EventCurrentStatus = EventManagement.Application.Models.Enums.EventCurrentStatus;
using EventType = EventManagement.Application.Models.Enums.EventType;
using PlaceType = EventManagement.Application.Models.Enums.PlaceType;
using EventCategory = EventManagement.Application.Models.Enums.EventCategory;

namespace Application.UnitTests.Handlers.EventFeatures
{
    public class ChangeVisibilityEventTests : HandlerBaseTests<ChangeVisibilityCommandHandler, ChangeVisibilityCommand,
        Response<string>>
    {
        private readonly Mock<IBackgroundService> _backgroundServiceMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;

        public ChangeVisibilityEventTests()
        {
            this._backgroundServiceMock = this._mocker.GetMock<IBackgroundService>();
            this._cacheServiceMock = this._mocker.GetMock<ICacheService>();
            this._eventRepositoryMock = this._mocker.GetMock<IEventRepository>();
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Request_Is_Null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.Handle((ChangeVisibilityCommand) null, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Should_Throw_BusinessRuleException_When_Bad_Status()
        {
            //Arrange
            var request = this._fixture.Create<ChangeVisibilityCommand>();

            this._eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>()))
                .ReturnsAsync(() => this.GetEventBase(EventCurrentStatus.Cancelled));

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(this._eventRepositoryMock.Object);

            //Act
            var result = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                this._handler.Handle(request, It.IsAny<CancellationToken>()));

            //Assert

            Assert.Equal(RuleErrorMessage.EventCannotBeCanceledRuleMessage, result.Message);
        }

        [Fact]
        public async Task Should_Throw_BusinessRuleException_When_No_Time_For_Modifications()
        {
            //Arrange
            var request = this._fixture.Create<ChangeVisibilityCommand>();

            this._eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>()))
                .ReturnsAsync(() => this.GetEventBase(startCustomDate: DateTime.Now.AddDays(this.GetRandomInt())));

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(this._eventRepositoryMock.Object);

            //Act
           var result = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                this._handler.Handle(request, It.IsAny<CancellationToken>()));

           //Assert

           Assert.Equal(RuleErrorMessage.TimeAllowsModificationRuleMessage, result.Message);
        }


        [Fact]
        public async Task Visibility_Should_Be_Active_While_Event_Was_Suspended()
        {
            //Arrange
            var request = this._fixture.Create<ChangeVisibilityCommand>();

            this._eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>()))
                .ReturnsAsync(() => this.GetEventBase(EventCurrentStatus.Suspended));

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(this._eventRepositoryMock.Object);

            //Act
            var result = await this._handler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            Assert.True(result.Success);
            this._backgroundServiceMock.Verify(v => v.DeleteCancelEventScheduleJob(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Visibility_Should_Be_Suspended_While_Event_Was_Active()
        {
            //Arrange
            var request = this._fixture.Create<ChangeVisibilityCommand>();

            this._eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>()))
                .ReturnsAsync(() => this.GetEventBase());

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(this._eventRepositoryMock.Object);

            //Act
            var result = await this._handler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            Assert.True(result.Success);
            this._backgroundServiceMock.Verify(v => v.DeleteCancelEventScheduleJob(It.IsAny<int>()), Times.Never);
            this._backgroundServiceMock.Verify(
                v => v.CancelEventWhenSuspendedScheduleJob(It.IsAny<CancelEventCommand>(), It.IsAny<TimeSpan>()),
                Times.Once);
        }

        private EventBaseDao GetEventBase(EventCurrentStatus status = EventCurrentStatus.Active, DateTime? startCustomDate = null)
        {
            var recurringEvent = this._fixture.Create<bool>();
            var city = "City_Test";
            var street = "Street_Test";
            var numberStreet = "123a";
            var postalCode = "0" + this.GetRandomInt() + "-" + this.GetRandomInt(100, 1000);
            var eventName = "Event_Test";
            var eventDescription = "Event_Description_Test";
            var startDate = startCustomDate ?? DateTime.Now.AddDays(this.GetRandomInt(20, 50));
            var endTime = startDate.AddDays(1);
            var place = PlaceType.Outdoors;
            var category = EventCategory.Massive;
            var eventType = EventType.Modern;

            var eventBaseDao = new EventBaseDao()
            {
                City = city,
                CurrentStatus = status,
                EndDate = endTime,
                StartDate = startDate,
                EventCategory = category,
                EventDescription = eventDescription,
                EventName = eventName,
                EventType = eventType,
                Id = this.GetRandomInt(),
                NumberStreet = numberStreet,
                PostalCode = postalCode,
                RecurringEvent = recurringEvent,
                Street = street,
                PlaceType = place
            };

            return eventBaseDao;
        }

        //private Event GetEventBase(bool isActive = true)
        //{
        //    var recurringEvent = this._fixture.Create<bool>();
        //    var city = "City_Test";
        //    var street = "Street_Test";
        //    var numberStreet = "123a";
        //    var postalCode = "0" + this.GetRandomInt() + "-" + this.GetRandomInt(100, 1000);
        //    var eventName = "Event_Test";
        //    var eventDescription = "Event_Description_Test";
        //    var startDate = DateTime.Now.AddDays(this.GetRandomInt(20, 50));
        //    var endTime = startDate.AddDays(1);
        //    var address = Address.Create(city, street, numberStreet, postalCode);
        //    var time = EventTime.Create(startDate, endTime);
        //    var name = EventName.Create(eventName);
        //    var eventStatus = isActive ? EventCurrentStatus.Active: EventCurrentStatus.Suspended;
        //    var description = EventDescription.Create(eventDescription);
        //    var place =
        //        Enumeration.GetById<EventManagement.Domain.Base.EnumerationClasses.PlaceType>((int)PlaceType.Outdoors);
        //    var category =
        //        Enumeration.GetById<EventManagement.Domain.Base.EnumerationClasses.EventCategory>(
        //            (int) EventCategory.Massive);
        //    var eventType =
        //        Enumeration.GetById<EventManagement.Domain.Base.EnumerationClasses.EventType>((int) EventType.Modern);
        //    var loadedEvent = Event.LoadEvent(GetRandomInt(), null, null, null, name, description, time, place,
        //        eventStatus, recurringEvent, address, category,
        //        eventType);

        //    return loadedEvent;
        //}
    }
}