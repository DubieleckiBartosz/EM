using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Commands.CancelEvent
{
    public class CancelEventCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }

        [JsonConstructor]
        public CancelEventCommand(int eventId)
        {
            this.EventId = eventId;
        }
    }
}
