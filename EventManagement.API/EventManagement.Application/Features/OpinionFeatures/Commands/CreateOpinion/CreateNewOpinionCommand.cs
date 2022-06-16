using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.OpinionFeatures.Commands.CreateOpinion
{
    public class CreateNewOpinionCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }
        public string Comment { get; set; }
        public int Stars { get; set; }

        [JsonConstructor]
        public CreateNewOpinionCommand(string comment, int stars, int eventId)
        {
            this.Comment = comment;
            this.Stars = stars;
            this.EventId = eventId;
        }
    }
}