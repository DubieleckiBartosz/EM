using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Attributes;
using EventManagement.Application.Contracts;
using EventManagement.Application.Helpers;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using MediatR;

namespace EventManagement.Application.Features.OpinionFeatures.Commands.RemoveOpinion
{
    [WithTransaction]
    public class RemoveOpinionCommandHandler : IRequestHandler<RemoveOpinionCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public RemoveOpinionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ICurrentUserService currentUserService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._currentUserService =
                currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<Response<string>> Handle(RemoveOpinionCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventDao =
                await this._unitOfWork.EventRepository.GetEventWithOpinionsAsync(request.EventId);
            var eventAggregate = this._mapper.Map<Event>(eventDao);
            var currentUserId = this._currentUserService.UserId.ToInt();
            var superAccess = this._currentUserService.HasSuperAccess();
            eventAggregate.RemoveOpinion(request.OpinionId, currentUserId, superAccess);

            await this._unitOfWork.CompleteAsync(eventAggregate);
            return Response<string>.Ok(ResponseStrings.OpinionRemoved);
        }
    }
}