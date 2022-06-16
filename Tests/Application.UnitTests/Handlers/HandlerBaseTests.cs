using System;
using AutoFixture;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Mappings;
using MediatR;
using Moq;
using Moq.AutoMock;

namespace Application.UnitTests.Handlers
{
    public abstract class HandlerBaseTests<THandler, TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        protected Mock<ICurrentUserService> _currentUserMock;
        protected Mock<IUnitOfWork> _unitOfWork;
        protected readonly Fixture _fixture;
        protected readonly AutoMocker _mocker;
        protected readonly IMapper _mapper;
        protected readonly THandler _handler;

        protected HandlerBaseTests()
        {
            this._mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EventProfile>();
            }).CreateMapper();

            this._mocker = new AutoMocker();
            this._fixture = new Fixture();
            this._currentUserMock = this._mocker.GetMock<ICurrentUserService>();
            this._unitOfWork = this._mocker.GetMock<IUnitOfWork>();
            this._mocker.Use(this._mapper);
            this._handler = this._mocker.CreateInstance<THandler>();
        }

        protected int GetRandomInt(int a = 1, int b = 10) => new Random().Next(a, b);

        protected string EventCannotBeCanceledRuleMessage = "Event is in canceled status.";

        protected string EventTimeCancelRuleEventHasAlreadyTakenPlaceMessage =
            "An event that has already taken place cannot be canceled.";

        protected string EventTimeCancelRuleCanceledDueToLackOfTimeMessage(int days) =>
            $"There are {days} days left before the event starts, so it's too late to cancel.";
    }
}