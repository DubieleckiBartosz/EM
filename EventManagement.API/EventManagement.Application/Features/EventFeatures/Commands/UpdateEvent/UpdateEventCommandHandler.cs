using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Features.EventFeatures.Commands.MarkAsRealizedEvent;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Commands.UpdateEvent
{
    [WithTransaction]
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBackgroundService _backgroundService;
        private readonly ICacheService _cacheService;

        public UpdateEventCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IBackgroundService backgroundService,
            ICacheService cacheService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._backgroundService = backgroundService ?? throw new ArgumentNullException(nameof(backgroundService));
            this._cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Response<string>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventRepository = this._unitOfWork.EventRepository;
            var eventDao = await eventRepository.GetEventBaseDataAsync(request.EventId);
            var aggregate = this._mapper.Map<Event>(eventDao);

            var startDate = request.StartDate?.ToDateTime();
            var endTime = request.EndDate?.ToDateTime();

            var time = EventTime.Create(startDate ?? eventDao.StartDate, endTime ?? eventDao.EndDate);
            var description = EventDescription.Create(request.EventDescription ?? eventDao.EventDescription);
            var place = Enumeration.GetById<PlaceType>(request.PlaceType?.EnumToInt() ??
                                                       eventDao.PlaceType.EnumToInt());
            var category =
                Enumeration.GetById<EventCategory>(request.CategoryType?.EnumToInt() ??
                                                   eventDao.EventCategory.EnumToInt());
            var eventType =
                Enumeration.GetById<EventType>(request.EventType?.EnumToInt() ?? eventDao.EventType.EnumToInt());
            var recurring = request.RecurringEvent;

            aggregate.Update(time, recurring, description, place, category, eventType);
            await eventRepository.UpdateAsync(aggregate);

            await this._cacheService.RemoveByKey(CacheKeys.EventsKey);

            await this._unitOfWork.CompleteAsync(aggregate);

            if (startDate == null || endTime == null)
            {
                return Response<string>.Ok(ResponseStrings.EventUpdated);
            }

            var setupBackgroundJobTime = (endTime.Value - DateTime.Now);

            this._backgroundService.DeleteChangeStatusToRealizedScheduleJob(aggregate.Id);
            this._backgroundService.ChangeStatusToRealizedScheduleJob(new MarkAsRealizedCommand(aggregate.Id),
                setupBackgroundJobTime);
            
            return Response<string>.Ok(ResponseStrings.EventUpdated);
        }
    }
}