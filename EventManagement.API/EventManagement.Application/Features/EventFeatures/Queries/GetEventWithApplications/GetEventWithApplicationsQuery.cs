using EventManagement.Application.Models.Dto;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventWithApplications
{
    public class GetEventWithApplicationsQuery : IRequest<Response<EventWithApplicationsDto>>
    {
        public int EventId { get; set; }

        [JsonConstructor]
        public GetEventWithApplicationsQuery(int eventId)
        {
            this.EventId = eventId;
        }
    }
}
