using System.Collections.Generic;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventProposalFeatures.Queries.GetProposals
{
    public class GetProposalsQuery : IRequest<Response<List<PerformanceProposalDto>>>
    {
    }
}
