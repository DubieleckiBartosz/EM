﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using EventManagement.Application.Strings;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Commands.ChangeVisibilityEvent
{
    [WithTransaction]
    public class ChangeVisibilityCommandHandler : IRequestHandler<ChangeVisibilityCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBackgroundService _backgroundService;
        private readonly ICacheService _cacheService;

        public ChangeVisibilityCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IBackgroundService backgroundService, ICacheService cacheService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._backgroundService = backgroundService ?? throw new ArgumentNullException(nameof(backgroundService));
            this._cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

        }

        public async Task<Response<string>> Handle(ChangeVisibilityCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventRepository = this._unitOfWork.EventRepository;
            var eventDao = await eventRepository.GetEventBaseDataAsync(request.EventId);
            var eventAggregate = this._mapper.Map<Event>(eventDao);

            eventAggregate.ChangeVisibility();
            await eventRepository.UpdateAsync(eventAggregate);

            if (eventAggregate.IsActive())
            {
                this._backgroundService.DeleteCancelEventScheduleJob(eventAggregate.Id);
            }
            else
            {
                var dateJob = eventAggregate.EventTime.StartDate.AddDays(-13);
                var timeForJob = dateJob - DateTime.Now;
                this._backgroundService.CancelEventWhenSuspendedScheduleJob(new CancelEventCommand(eventAggregate.Id),
                    timeForJob);
            }

            await this._cacheService.RemoveByKey(CacheKeys.EventsKey);
          
            await this._unitOfWork.CompleteAsync(eventAggregate);
          
            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }
    }
}