using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Features.Search;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventWithOpinions
{
    public class
        GetEventWithOpinionsQueryHandler : IRequestHandler<GetEventWithOpinionsQuery, Response<EventWithOpinionsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetEventWithOpinionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<EventWithOpinionsDto>> Handle(GetEventWithOpinionsQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request?.SortModel == null)
            {
                request.SortModel = new SortModelQuery();
            }

            var sortType = string.IsNullOrWhiteSpace(request.SortModel.Type) ? "desc" : request.SortModel.Type;
            var sortName = string.IsNullOrWhiteSpace(request.SortModel.Name) ? "Id" : request.SortModel.Name;
            sortType = sortType.ToLower() == "desc" ? "desc" : "asc";

            var result =
                await this._unitOfWork.EventRepository.GetEventWithOpinionsAsync(request.EventId, sortName,
                    sortType);

            var resultMap = this._mapper.Map<EventWithOpinionsDto>(result);
            return Response<EventWithOpinionsDto>.Ok(resultMap);
        }
    }
}