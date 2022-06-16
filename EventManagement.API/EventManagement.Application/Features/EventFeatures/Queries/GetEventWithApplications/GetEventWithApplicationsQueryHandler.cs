using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventWithApplications
{
    public class GetEventWithApplicationsQueryHandler : IRequestHandler<GetEventWithApplicationsQuery, Response<EventWithApplicationsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetEventWithApplicationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<Response<EventWithApplicationsDto>> Handle(GetEventWithApplicationsQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventRepository = this._unitOfWork.EventRepository;
            var eventDao = await eventRepository.GetEventWithApplicationsAsync(request.EventId);

            var eventDto = this._mapper.Map<EventWithApplicationsDto>(eventDao);
            return Response<EventWithApplicationsDto>.Ok(eventDto);
        }
    }
}
