using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Strings;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Commands.MarkAsRealizedEvent
{
    public class MarkAsRealizedCommandHandler : IRequestHandler<MarkAsRealizedCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggerManager<MarkAsRealizedCommandHandler> _loggerManager;
        private readonly ICacheService _cacheService;

        public MarkAsRealizedCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ILoggerManager<MarkAsRealizedCommandHandler> loggerManager, ICacheService cacheService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
            this._cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Unit> Handle(MarkAsRealizedCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventRepository = this._unitOfWork.EventRepository;
            var eventDao = await eventRepository.GetEventBaseDataAsync(request.EventId);
            var eventAggregate = this._mapper.Map<Event>(eventDao);

            if (eventAggregate.CurrentStatus.Id != EventCurrentStatus.Active.Id)
            {
                this._loggerManager.LogError(null,
                    $"You cannot update to 'Realized' if the status of the event is not active.");
                return Unit.Value;
            }

            eventAggregate.ChangeStatusToRealized();

            await eventRepository.UpdateAsync(eventAggregate);

            this._loggerManager.LogInformation(null, $"Event {request.EventId} status has been changed to realized.");

            await this._cacheService.RemoveByKey(CacheKeys.EventsKey);

            return Unit.Value;
        }
    }
}