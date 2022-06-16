using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventFeatures.Commands.ChangeVisibilityEvent
{
    public class ChangeVisibilityCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }

        [JsonConstructor]
        public ChangeVisibilityCommand(int eventId)
        {
            this.EventId = eventId;
        }
    }
}