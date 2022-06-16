using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EventManagement.Application.Contracts;
using EventManagement.Application.Helpers;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventProposalFeatures.Queries.GetProposals
{
    public class
        GetProposalsQueryHandler : IRequestHandler<GetProposalsQuery,
            Response<List<PerformanceProposalDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public GetProposalsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ICurrentUserService currentUserService)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._currentUserService =
                currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Response<List<PerformanceProposalDto>>> Handle(GetProposalsQuery request,
            CancellationToken cancellationToken)
        {
            var hasSuperAccess = this._currentUserService.HasSuperAccess();
            var currentUserId = this._currentUserService.UserId.ToInt();
            var proposalRepository = this._unitOfWork.ProposalRepository;

            var result = await proposalRepository.GetProposalsAsync(hasSuperAccess, userId: currentUserId);
            var resultMap = this._mapper.Map<List<PerformanceProposalDto>>(result);
            return Response<List<PerformanceProposalDto>>.Ok(resultMap);
        }
    }
}