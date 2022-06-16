using EventManagement.Application.Models.Enums;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventApplicationFeatures.Commands.CreateEventApplication
{
    public class CreateApplicationCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }
        public TypePerformance TypePerformance { get; set; }
        public int DurationInMinutes { get; set; }

        [JsonConstructor]
        public CreateApplicationCommand(int eventId,
            TypePerformance typePerformance, int durationInMinutes)
        {
            this.EventId = eventId;
            this.TypePerformance = typePerformance;
            this.DurationInMinutes = durationInMinutes;
        }
    }
}
