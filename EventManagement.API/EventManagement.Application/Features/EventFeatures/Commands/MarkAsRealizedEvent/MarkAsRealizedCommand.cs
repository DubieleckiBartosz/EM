using MediatR;

namespace EventManagement.Application.Features.EventFeatures.Commands.MarkAsRealizedEvent
{
    public class MarkAsRealizedCommand : IRequest
    {
        public int EventId { get; set; }

        public MarkAsRealizedCommand(int eventId)
        {
            this.EventId = eventId;
        }
    }
}
