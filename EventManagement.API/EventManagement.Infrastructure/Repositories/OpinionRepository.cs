using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Models.Dao;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.Repositories
{
    public class OpinionRepository : BaseRepository, IOpinionRepository
    {
        public OpinionRepository(EventContext context) : base(context)
        {
        }


        public async Task<EventOpinionDao> GetOpinionAsync(int opinionId)
        {
            var param = new DynamicParameters();
            param.Add("@opinionId", opinionId);

            var result = (await this.QueryAsync<EventOpinionDao>("opinion_getById_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result;
        }

        public async Task CreateNewOpinionAsync(EventOpinion opinion)
        {
            var param = new DynamicParameters();
            param.Add("@eventId", opinion.EventId);
            param.Add("@userId", opinion.UserId);
            param.Add("@comment", opinion.Comment.Comment);
            param.Add("@stars", opinion.Stars);

            var result = await this.ExecuteAsync("opinion_createNewOpinion_I", param,
                commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.CreationNewOpinionFailed);
            }
        }

        public async Task UpdateOpinionAsync(EventOpinion opinion)
        {
            var param = new DynamicParameters();

            param.Add("@opinionId", opinion.Id);
            param.Add("@userId", opinion.UserId);
            param.Add("@comment", opinion.Comment.Comment);
            param.Add("@stars", opinion.Stars);

            var result = await this.ExecuteAsync("opinion_updateOpinion_U", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.CreationNewOpinionFailed);
            }
        }

        public async Task RemoveOpinion(int opinionId)
        {
            var param = new DynamicParameters();

            //param.Add("@isAdminOrOwner", isAdminOrOwner);
            //param.Add("@userId", userId);
            param.Add("@opinionId", opinionId);

            var result = await this.ExecuteAsync("opinion_removeOpinion_D", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.CreationNewOpinionFailed);
            }
        }
    }
}