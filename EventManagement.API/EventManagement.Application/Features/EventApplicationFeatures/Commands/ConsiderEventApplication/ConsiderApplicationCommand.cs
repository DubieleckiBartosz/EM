using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventApplicationFeatures.Commands.ConsiderEventApplication
{
    public class ConsiderApplicationCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }
        public int ApplicationId { get; set; }
        public bool Approved { get; set; }

        [JsonConstructor]
        public ConsiderApplicationCommand(int eventId, int applicationId, bool approved)
        {
            this.EventId = eventId;
            this.ApplicationId = applicationId;
            this.Approved = approved;
        }
    }
}
