using System;
using System.Threading;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventProposalFeatures.Commands.RemoveProposal
{
    public class RemoveProposalCommandHandler : IRequestHandler<RemoveProposalCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveProposalCommandHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Response<string>> Handle(RemoveProposalCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var proposalRepository = this._unitOfWork.ProposalRepository;

            await proposalRepository.RemoveProposalAsync(request.ProposalId);
            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }
    }
}