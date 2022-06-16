using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Base.EnumerationClasses;
using EventManagement.Domain.Entities;
using MediatR;

namespace EventManagement.Application.Features.EventApplicationFeatures.Commands.CreateEventApplication
{
    [WithTransaction]
    public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateApplicationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._currentUserService =
                currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<string>> Handle(CreateApplicationCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var typePerformance = Enumeration.GetById<TypePerformance>((int) request.TypePerformance);
            var durationInMinutes = request.DurationInMinutes;
            var currentUserId = this._currentUserService.UserId.ToInt();
            var eventRepository = this._unitOfWork.EventRepository;
            var performerRepository = this._unitOfWork.Performer;

            var performerDao = await performerRepository.GetPerformerByIdAsync(currentUserId);
            var eventDao = await eventRepository.GetEventWithApplicationsAsync(request.EventId);

            var performerAggregate = this._mapper.Map<Performer>(performerDao);
            var eventAggregate = this._mapper.Map<Event>(eventDao);
            eventAggregate.CreateNewApplication(performerAggregate, typePerformance, durationInMinutes);

            await this._unitOfWork.CompleteAsync(eventAggregate);
            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }
    }
}