using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using EventManagement.Application.Models.Dao.EventDAOs;
using EventManagement.Application.Models.Enums;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Base;
using Moq;
using Xunit;

namespace Application.UnitTests.Handlers.EventFeatures
{
    public class CancelEventTests : HandlerBaseTests<CancelEventCommandHandler, CancelEventCommand, Response<string>>
    {
        [Fact]
        public async Task Should_throw_Exception_When_Request_Is_Null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.Handle(null, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Event_Has_Cancelled_Status()
        {
            var request = this._fixture.Create<CancelEventCommand>();
            var eventBaseData = this._fixture.Build<EventBaseDao>()
                .With(w => w.CurrentStatus, EventCurrentStatus.Cancelled)
                .With(w => w.City, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.Street, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.NumberStreet, this._fixture.Create<string>().Substring(3, 10))
                .With(w => w.PostalCode, this._fixture.Create<string>().Substring(3, 10)).Create();

            var eventRepositoryMock = this._mocker.GetMock<IEventRepository>();

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>())).ReturnsAsync(eventBaseData);

            var result = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                this._handler.Handle(request, It.IsAny<CancellationToken>()));

            Assert.Equal(this.EventCannotBeCanceledRuleMessage, result.Message);
        }


        [Fact]
        public async Task Should_Throw_Exception_When_Event_Cannot_Be_Canceled_Once_It_Has_Happened()
        {
            var request = this._fixture.Create<CancelEventCommand>();
            var eventBaseData = this._fixture.Build<EventBaseDao>()
                .With(w => w.StartDate, DateTime.Now.AddDays(-this._fixture.Create<int>()))
                .With(w => w.CurrentStatus, EventCurrentStatus.Realized)
                .With(w => w.City, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.Street, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.NumberStreet, this._fixture.Create<string>().Substring(3, 10))
                .With(w => w.PostalCode, this._fixture.Create<string>().Substring(3, 10)).Create();

            var eventRepositoryMock = this._mocker.GetMock<IEventRepository>();

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>())).ReturnsAsync(eventBaseData);

            var result = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                this._handler.Handle(request, It.IsAny<CancellationToken>()));

            Assert.Equal(this.EventTimeCancelRuleEventHasAlreadyTakenPlaceMessage, result.Message);
        }


        [Fact]
        public async Task Should_Throw_Exception_When_Event_Cannot_Be_Canceled_Due_To_Lack_Of_Time()
        {
            var request = this._fixture.Create<CancelEventCommand>();
            var eventBaseData = this._fixture.Build<EventBaseDao>()
                .With(w => w.StartDate, DateTime.Now.AddDays(this.GetRandomInt()))
                .With(w => w.CurrentStatus, EventCurrentStatus.Active)
                .With(w => w.City, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.Street, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.NumberStreet, this._fixture.Create<string>().Substring(3, 10))
                .With(w => w.PostalCode, this._fixture.Create<string>().Substring(3, 10)).Create();
            var daysToStart = (eventBaseData.StartDate - DateTime.Now).Days;

            var eventRepositoryMock = this._mocker.GetMock<IEventRepository>();

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>())).ReturnsAsync(eventBaseData);

            var result = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                this._handler.Handle(request, It.IsAny<CancellationToken>()));

            Assert.Equal(this.EventTimeCancelRuleCanceledDueToLackOfTimeMessage(daysToStart), result.Message);
        }

        [Fact]
        public async Task Cancellation_Event_Should_Pass()
        {
            var request = this._fixture.Create<CancelEventCommand>();
            var eventBaseData = this._fixture.Build<EventBaseDao>()
                .With(w => w.StartDate, DateTime.Now.AddDays(this.GetRandomInt(15, 20)))
                .With(w => w.CurrentStatus, EventCurrentStatus.Active)
                .With(w => w.City, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.Street, this._fixture.Create<string>().Substring(10, 20))
                .With(w => w.NumberStreet, this._fixture.Create<string>().Substring(3, 10))
                .With(w => w.PostalCode, this._fixture.Create<string>().Substring(3, 10)).Create();

            var backgroundRepositoryMock = this._mocker.GetMock<IBackgroundService>();
            var eventRepositoryMock = this._mocker.GetMock<IEventRepository>();

            this._unitOfWork.SetupGet(_ => _.EventRepository).Returns(eventRepositoryMock.Object);
            eventRepositoryMock.Setup(_ => _.GetEventBaseDataAsync(It.IsAny<int>())).ReturnsAsync(eventBaseData);

            var result = await this._handler.Handle(request, It.IsAny<CancellationToken>());

            Assert.Equal(ResponseStrings.OperationSuccess, result.Message);
            backgroundRepositoryMock.Verify(v => v.DeleteCancelEventScheduleJob(It.IsAny<int>()), Times.Once);

        }
    }
}

