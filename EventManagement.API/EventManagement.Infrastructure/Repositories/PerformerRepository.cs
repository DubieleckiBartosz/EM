using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Models.Dao.PerformerDAOs;
using EventManagement.Application.Models.Dao.ProposalDAOs;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.Repositories
{
    public class PerformerRepository : BaseRepository, IPerformerRepository
    {
        public PerformerRepository(EventContext context) : base(context)
        {
        }

        public async Task<PerformerDao> GetPerformerByIdAsync(int userId)
        {
            var param = new DynamicParameters();

            param.Add("@userId", userId);
            var result = (await this.QueryAsync<PerformerDao>("performer_getByUserId_S", param,
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result;
        }

        public async Task<PerformerWithProposalsDao> GetPerformerWithProposalsAsync(int performerId)
        {
            var param = new DynamicParameters();

            param.Add("@performerId", performerId);

            var performerDict = new Dictionary<int, PerformerWithProposalsDao>();

            var result = (await this.QueryAsync<PerformerWithProposalsDao, ProposalDao, PerformerWithProposalsDao>(
                "performer_getPerformerWithProposals_S", (pp, p) =>
                {
                    PerformerWithProposalsDao performer;
                    if (!performerDict.TryGetValue(pp.Id, out performer))
                    {
                        performer = pp;
                        performer.Proposals = new List<ProposalDao>();
                        performerDict.Add(pp.Id, pp);
                    }

                    if (p != null)
                    {
                        performer.Proposals.Add(p);
                    }

                    return performer;
                }, splitOn: "Id,Id", param, this.Transaction,
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result;
        }

        public async Task<List<PerformerDao>> GetPerformersByEventIdAsync(int eventId)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);
            var result = (await this.QueryAsync<PerformerDao>("performer_getPerformersByEventId_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure))?.ToList();

            return result;
        }

        public async Task CreatePerformerAsync(Performer performer)
        {
            var param = new DynamicParameters();

            param.Add("@userId", performer.UserId);
            param.Add("@performerName", performer.PerformerName.NameValue);
            param.Add("@vip", performer.VIP);
            param.Add("@performerMail", performer.PerformerMail);
            param.Add("@numberOfPeople", performer.NumberOfPeople);

            var result = await this.ExecuteAsync("performer_createNewPerformer_I", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.CreatePerformerFailed);
            }
        }

        public async Task UpdatePerformerAsync(Performer performer)
        {
            var param = new DynamicParameters();

            param.Add("@performerId", performer.Id);
            param.Add("@numberOfPeople", performer.NumberOfPeople);
            param.Add("@performerMail", performer.PerformerMail);

            var result = await this.ExecuteAsync("performer_updatePerformer_U", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.UpdateFailed);
            }
        }

        public async Task<List<PerformerWithNumberOfPerformancesDao>> GetPerformersBySearchAsync(int pageNumber,
            int pageSize, string sortName,
            string sortType, string performerName, bool? vip)
        {
            var param = new DynamicParameters();

            param.Add("@sortName", sortName);
            param.Add("@vip", vip);
            param.Add("@sortType", sortType);
            param.Add("@performerName", performerName);

            var result = await this.QueryAsync<PerformerWithNumberOfPerformancesDao>(
                "performer_getPerformersWithNumberPerformancesBySearch_S",
                param, this.Transaction, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result?.Skip((pageNumber - 1) * pageSize)?.Take(pageSize)?.ToList();
        }
        
        
        public async Task<List<PerformerDao>> GetAllPerformersAsync()
        {
            var transaction = this.Transaction;
            var result = await this.QueryAsync<PerformerDao>(
                "performer_getAllPerformers_S", transaction: transaction, commandType: CommandType.StoredProcedure);
        
            return result?.ToList();
        }
    }
}