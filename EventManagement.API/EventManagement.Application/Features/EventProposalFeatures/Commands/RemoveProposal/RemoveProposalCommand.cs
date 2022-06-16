using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.EventProposalFeatures.Commands.RemoveProposal
{
    public class RemoveProposalCommand : IRequest<Response<string>>
    {
        public int ProposalId { get; set; }

        [JsonConstructor]
        public RemoveProposalCommand(int proposalId)
        {
            this.ProposalId = proposalId;
        }
    }
}
