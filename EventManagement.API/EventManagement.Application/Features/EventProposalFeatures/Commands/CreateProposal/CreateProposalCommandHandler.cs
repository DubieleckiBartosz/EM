using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Strings.Responses;
using EventManagement.Application.Wrappers;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;
using MediatR;

namespace EventManagement.Application.Features.EventProposalFeatures.Commands.CreateProposal
{
    public class CreateProposalCommandHandler : IRequestHandler<CreateProposalCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProposalCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<string>> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var eventRepository = this._unitOfWork.EventRepository;
            var performerRepository = this._unitOfWork.Performer;
            var performanceProposalRepository = this._unitOfWork.ProposalRepository;

            var eventDao = await eventRepository.GetEventBaseDataAsync(request.EventId);


            if (eventDao.StartDate < request.ActiveTo)
            {
                throw new ArgumentException(ResponseStrings.OldDateForProposal);
            }

            var performerDao = await performerRepository.GetPerformerWithProposalsAsync(request.PerformerId);
            var performer = this._mapper.Map<Performer>(performerDao);

            var message = Message.CreateMessage(request.Message);
            var newProposalResult = performer.AddNewProposal(request.EventId, message, request.ActiveTo);
            await performanceProposalRepository.CreateProposalAsync(newProposalResult);

            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }
    }
}