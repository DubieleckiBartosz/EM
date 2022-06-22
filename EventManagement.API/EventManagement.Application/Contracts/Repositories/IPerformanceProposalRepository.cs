using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Models.Dao.ProposalDAOs;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Contracts.Repositories
{
    public interface IPerformanceProposalRepository
    {
        Task<List<ProposalDao>> GetProposalsByPerformerIdAsync(int performerId);
        Task<List<ProposalDao>> GetProposalsAsync(bool hasSuperAccess, int userId);
        Task CreateProposalAsync(PerformanceProposal proposal);
        Task RemoveProposalAsync(int proposalId);
        Task RemoveExpiredProposalsAsync();
        Task RemoveAllProposals(int eventId);
    }
}
