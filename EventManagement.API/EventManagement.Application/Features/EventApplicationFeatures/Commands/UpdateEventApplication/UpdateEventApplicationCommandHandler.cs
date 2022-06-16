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

namespace EventManagement.Application.Features.EventApplicationFeatures.Commands.UpdateEventApplication
{
    [WithTransaction]

    public class UpdateEventApplicationCommandHandler : IRequestHandler<UpdateEventApplicationCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateEventApplicationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._currentUserService =
                currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<string>> Handle(UpdateEventApplicationCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventApplicationRepository = this._unitOfWork.EventApplication;
            var eventRepository = this._unitOfWork.EventRepository;

            var superAccess = this._currentUserService.HasSuperAccess();
            var userId = this._currentUserService.UserId.ToInt();

            var eventApplicationDao =
                await eventApplicationRepository.GetApplicationByIdAsync(request.ApplicationId, userId, superAccess);

            var eventApplicationEntity = this._mapper.Map<EventApplication>(eventApplicationDao);

            var performance = request.TypePerformance == null
                ? null
                : Enumeration.GetById<TypePerformance>((int) request.TypePerformance);

            var isApplicant = !superAccess;
            eventApplicationEntity.Update(request.DurationInMinutes, performance, isApplicant);

            var result = await eventRepository.GetEventWithApplicationsAsync(request.EventId);

            var eventAggregate = this._mapper.Map<Event>(result);
            eventAggregate.UpdateApplication(eventApplicationEntity);

            await this._unitOfWork.CompleteAsync(eventAggregate);
            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }
    }
}