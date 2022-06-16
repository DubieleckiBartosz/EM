using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Features.Search;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.PerformerFeatures.Queries.GetPerformersBySearch
{
    public class GetPerformersBySearchQueryHandler : IRequestHandler<GetPerformersBySearchQuery,
        Response<List<PerformerWithNumberOfPerformancesDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetPerformersBySearchQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Response<List<PerformerWithNumberOfPerformancesDto>>> Handle(
            GetPerformersBySearchQuery request, CancellationToken cancellationToken)
        {
            request ??= new GetPerformersBySearchQuery();

            if (request?.SortModel == null)
            {
                request.SortModel = new SortModelQuery();
            }

            var sortType = string.IsNullOrWhiteSpace(request.SortModel.Type) ? "desc" : request.SortModel.Type;
            var sortName = string.IsNullOrWhiteSpace(request.SortModel.Name)
                ? "NumberPerformance"
                : request.SortModel.Name;
            sortType = sortType.ToLower() == "desc" ? "desc" : "asc";

            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;
            var vip = request.OnlyVip;
            var performerName = request.PerformerName;
            var performerRepository = this._unitOfWork.Performer;

            var result =
                await performerRepository.GetPerformersBySearchAsync(pageNumber, pageSize, sortName, sortType,
                    performerName, vip);

            var resultMap = this._mapper.Map<List<PerformerWithNumberOfPerformancesDto>>(result);

            return Response<List<PerformerWithNumberOfPerformancesDto>>.Ok(resultMap);
        }
    }
}
