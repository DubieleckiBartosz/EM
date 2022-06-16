using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Features.Search;
using EventManagement.Application.Helpers;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventApplicationFeatures.Queries.GetEventApplicationsBySearch
{
    public class GetEventApplicationsQueryHandler : IRequestHandler<GetEventApplicationsQuery,
        Response<List<EventApplicationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetEventApplicationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ICurrentUserService currentUserService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._currentUserService =
                currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<Response<List<EventApplicationDto>>> Handle(GetEventApplicationsQuery request,
            CancellationToken cancellationToken)
        {
            request ??= new GetEventApplicationsQuery();

            if (request?.SortModel == null)
            {
                request.SortModel = new SortModelQuery();
            }

            var sortType = string.IsNullOrWhiteSpace(request.SortModel.Type) ? "desc" : request.SortModel.Type;
            var sortName = string.IsNullOrWhiteSpace(request.SortModel.Name) ? "Id" : request.SortModel.Name;
            sortType = sortType.ToLower() == "desc" ? "desc" : "asc";

            var performanceType = request.PerformanceType;
            var status = request.Status;
            var eventName = request.EventName;
            var from = request.From;
            var to = request.To;
            var durationInMinutesMin = request.DurationInMinutesMin;
            var durationInMinutesMax = request.DurationInMinutesMax;
            var lastModifiedByApplicant = request.LastModifiedByApplicant;

            var userId = this._currentUserService.UserId.ToInt();
            var hasSuperAccess = this._currentUserService.HasSuperAccess();

            var eventApplicationRepository = this._unitOfWork.EventApplication;
            var result = await eventApplicationRepository.GetEventApplicationsBySearchAsync(hasSuperAccess, userId,
                sortName, sortType, status, eventName, from, to, performanceType, durationInMinutesMin,
                durationInMinutesMax, lastModifiedByApplicant);

            var resultMap = this._mapper.Map<List<EventApplicationDto>>(result);
            return Response<List<EventApplicationDto>>.Ok(resultMap);
        }
    }
}