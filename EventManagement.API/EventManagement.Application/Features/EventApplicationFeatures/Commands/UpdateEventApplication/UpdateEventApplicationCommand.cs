using EventManagement.Application.Models.Enums;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventApplicationFeatures.Commands.UpdateEventApplication
{
    public class UpdateEventApplicationCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }
        public int ApplicationId { get; set; }
        public TypePerformance? TypePerformance { get; set; }
        public int? DurationInMinutes { get; set; }
    }
}