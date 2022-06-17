using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventDetails
{
    public class GetEventDetailsQueryHandler : IRequestHandler<GetEventDetailsQuery, Response<EventDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetEventDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ICurrentUserService currentUserService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._currentUserService =
                currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<Response<EventDetailsDto>> Handle(GetEventDetailsQuery request,
            CancellationToken cancellationToken)
        {
            var isOwner = this._currentUserService.IsOwner();
            var isAdmin = this._currentUserService.IsAdmin();
            var eventRepository = this._unitOfWork.EventRepository;
           
            var result = await eventRepository.GetEventDetailsAsync(request.EventId, isAdmin, isOwner);

            var resultMap = this._mapper.Map<EventDetailsDto>(result);

            return Response<EventDetailsDto>.Ok(resultMap);
        }
    }
}