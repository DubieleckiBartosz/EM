using EventManagement.Application.Models.Dto;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Queries.GetEventDetails
{
    public class GetEventDetailsQuery : IRequest<Response<EventDetailsDto>>
    {
        public int EventId { get; }

        [JsonConstructor]
        public GetEventDetailsQuery(int eventId)
        {
            this.EventId = eventId;
        }
    }
}