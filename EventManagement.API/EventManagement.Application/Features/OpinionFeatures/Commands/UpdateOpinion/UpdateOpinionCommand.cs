using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.OpinionFeatures.Commands.UpdateOpinion
{
    public class UpdateOpinionCommand : IRequest<Response<string>>
    {
        public int OpinionId { get; set; }
        public int? Stars { get; set; }
        public string Comment { get; set; }

        [JsonConstructor]
        public UpdateOpinionCommand(int? stars, string comment)
        {
            this.Stars = stars;
            this.Comment = comment;
        }
    }
}