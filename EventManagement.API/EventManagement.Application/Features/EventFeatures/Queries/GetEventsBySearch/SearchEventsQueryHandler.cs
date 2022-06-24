using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Features.Search;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Strings;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventsBySearch
{
    public class SearchEventsQueryHandler : IRequestHandler<SearchEventsQuery, ResponseList<EventBaseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public SearchEventsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<ResponseList<EventBaseDto>> Handle(SearchEventsQuery request,
            CancellationToken cancellationToken)
        {
            var fromCache = request == null;
            if (!fromCache)
            {
                var propertyInfos = request.GetType().GetProperties();
                foreach (var prop in propertyInfos)
                {
                    if (prop.Name is nameof(BaseSearchQuery.PageSize) or nameof(BaseSearchQuery.PageNumber))
                    {
                        continue;
                    }

                    fromCache = prop.GetValue(request, null) == default;
                    if (!fromCache)
                    {
                        break;
                    }
                }
            }

            if (fromCache)
            {
                var resultFromCache = await this.GetFromCache();
                if (resultFromCache != null)
                {
                    return resultFromCache;
                }
            }

            request ??= new SearchEventsQuery();

            if (request?.SortModel == null)
            {
                request.SortModel = new SortModelQuery();
            }

            var sortType = string.IsNullOrWhiteSpace(request.SortModel.Type) ? "desc" : request.SortModel.Type;
            var sortName = string.IsNullOrWhiteSpace(request.SortModel.Name) ? "EventId" : request.SortModel.Name;
            sortType = sortType.ToLower() == "desc" ? "desc" : "asc";

            var eventId = request.EventId;
            var eventName = request.EventName;
            var from = request.From;
            var to = request.To;
            var city = request.City;
            var placeType = request.PlaceType;
            var recurringEvent = request.RecurringEvent;
            var category = request.Category;
            var eventType = request.EventType;
            var eventCurrentStatus = request.EventCurrentStatus;
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;

            var eventRepository = this._unitOfWork.EventRepository;
            var result = await eventRepository.SearchEventsAsync(eventId, eventName, from, to, city, placeType,
                recurringEvent, category, eventType, eventCurrentStatus, sortName, sortType, pageNumber,
                pageSize);

            var count = result[0].Count;
            var resultMap = this._mapper.Map<List<EventBaseDto>>(result);

            var response = ResponseList<EventBaseDto>.Ok(count, resultMap);

            if (fromCache)
            {
                await this._cacheService.SetAsync(CacheKeys.EventsKey, response);
            }

            return response;
        }

        private async Task<ResponseList<EventBaseDto>> GetFromCache()
        {
            var result = await this._cacheService.GetAsync<ResponseList<EventBaseDto>>(CacheKeys.EventsKey);
            if (result != null && result.Data?.Any() == true)
            {
                return result;
            }

            return null;
        }
    }
}