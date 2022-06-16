using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Models.Dao.ProposalDAOs;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.Repositories
{
    public class PerformanceProposalRepository : BaseRepository, IPerformanceProposalRepository
    {
        public PerformanceProposalRepository(EventContext context) : base(context)
        {
        }

        public async Task<List<ProposalDao>> GetProposalsByPerformerIdAsync(int performerId)
        {
            var param = new DynamicParameters();

            param.Add("@performerId", performerId);

            var result = await this.QueryAsync<ProposalDao>("proposal_getPerformanceProposalsByPerformer_S", param, this.Transaction,
                commandType: CommandType.StoredProcedure);
        
            return result.ToList();
        }

        public async Task<List<ProposalDao>> GetProposalsAsync(bool hasSuperAccess, int userId)
        {
            var param = new DynamicParameters();

            param.Add("@hasSuperAccess", hasSuperAccess);
            param.Add("@userId", userId);

           var result = await this.QueryAsync<ProposalDao>("proposal_getPerformanceProposalsByUser_S", param, this.Transaction,
                commandType: CommandType.StoredProcedure);
           if (result == null)
           {
               throw new ArgumentNullException(ResponseStrings.DataNotFound);
           }

           return result.ToList();
        }

        public async Task CreateProposalAsync(PerformanceProposal proposal)
        {
            var param = new DynamicParameters();

            param.Add("@performerId", proposal.PerformerId);
            param.Add("@message", proposal.Message.MessageValue);
            param.Add("@activeTo", proposal.ActiveTo);
            param.Add("@eventId", proposal.EventId);

            await this.ExecuteAsync("proposal_createProposal_I", param, this.Transaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task RemoveProposalAsync(int proposalId)
        {
            var param = new DynamicParameters();

            param.Add("@proposalId", proposalId);

            var result = await this.ExecuteAsync("proposal_removeProposal_D", param, this.Transaction,
                commandType: CommandType.StoredProcedure);

            if (result <= 0)
            {
                throw new ArgumentNullException(ResponseStrings.OperationFailed);
            }
        }
        
        public async Task RemoveExpiredProposalsAsync()
        {
            await this.ExecuteAsync("proposal_removeExpiredProposals_D", transaction: this.Transaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}