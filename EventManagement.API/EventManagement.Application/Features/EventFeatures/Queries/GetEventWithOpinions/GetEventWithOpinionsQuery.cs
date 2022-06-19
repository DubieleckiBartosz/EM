using EventManagement.Application.Features.Search;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventWithOpinions
{
    public class GetEventWithOpinionsQuery: IRequest<Response<EventWithOpinionsDto>>
    {
        public int EventId { get; set; }
        public SortModelQuery SortModel { get; set; }

        [JsonConstructor]
        public GetEventWithOpinionsQuery(int eventId, SortModelQuery sortModel = null)
        {
            this.EventId = eventId;
            this.SortModel = sortModel;
        }
    }
}