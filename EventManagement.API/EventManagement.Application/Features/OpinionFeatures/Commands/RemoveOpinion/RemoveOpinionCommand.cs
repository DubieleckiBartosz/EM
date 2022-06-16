using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.OpinionFeatures.Commands.RemoveOpinion
{
    public class RemoveOpinionCommand : IRequest<Response<string>>
    {
        public int EventId { get; set; }
        public int OpinionId { get; set; }
    }
}