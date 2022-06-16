using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Features.EventFeatures.Commands.MarkAsRealizedEvent;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Commands.CreateEvent
{
    [WithTransaction]
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Response<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundService _backgroundService;

        public CreateEventCommandHandler(IUnitOfWork unitOfWork, IBackgroundService backgroundService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._backgroundService = backgroundService ?? throw new ArgumentNullException(nameof(backgroundService));
        }

        public async Task<Response<int>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var startDate = request.StartDate.ToDateTime();
            var endTime = request.EndDate.ToDateTime();
            var address = Address.Create(request.City, request.Street, request.NumberStreet, request.PostalCode);
            var time = EventTime.Create(startDate, endTime);
            var name = EventName.Create(request.EventName);
            var description = EventDescription.Create(request.EventDescription);
            var place = Enumeration.GetById<PlaceType>(request.PlaceType.EnumToInt());
            var category = Enumeration.GetById<EventCategory>(request.CategoryType.EnumToInt());
            var eventType = Enumeration.GetById<EventType>(request.EventType.EnumToInt());
            var newEvent = Event.Create(name, description, time, place, request.RecurringEvent, address, category,
                eventType);

            var eventRepository = this._unitOfWork.EventRepository;
            var eventIdentifier = await eventRepository.CreateEventAsync(newEvent);
            await this._unitOfWork.CompleteAsync(newEvent);

            var setupBackgroundJobTime = (newEvent.EventTime.EndDate - DateTime.Now).Minutes;

            this._backgroundService.ChangeStatusToRealizedScheduleJob(new MarkAsRealizedCommand(eventIdentifier),
                TimeSpan.FromMinutes(setupBackgroundJobTime));

            return Response<int>.Ok(eventIdentifier, ResponseStrings.EventCreated);
        }
    }
}