using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;
using EventManagement.Application.Models.Dao.PerformerDAOs;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.Repositories
{
    public class EventApplicationRepository : BaseRepository, IEventApplicationRepository
    {
        public EventApplicationRepository(EventContext eventContext) : base(eventContext)
        {
        }

        public async Task<List<EventApplicationDao>> GetEventApplicationsBySearchAsync(bool hasSuperAccess, int userId,
            string sortName, string sortType,
            int? status = null, string eventName = null, DateTime? from = null, DateTime? to = null,
            int? performanceType = null,
            int? durationInMinutesMin = null, int? durationInMinutesMax = null, bool? lastModifiedByApplicant = null)
        {
            var param = new DynamicParameters();

            param.Add("@sortName", sortName);
            param.Add("@sortType", sortType);
            param.Add("@status", status);
            param.Add("@eventName", eventName);
            param.Add("@from", from);
            param.Add("@to", to);
            param.Add("@performanceType", performanceType);
            param.Add("@durationInMinutesMin", durationInMinutesMin);
            param.Add("@durationInMinutesMax", durationInMinutesMax);
            param.Add("@lastModifiedByApplicant", lastModifiedByApplicant);
            param.Add("@hasSuperAccess", hasSuperAccess);
            param.Add("@userId", userId);

            var result = await this.QueryAsync<EventApplicationDao>("application_getApplicationsBySearch_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result.ToList();
        }

        public async Task<EventApplicationDao> GetApplicationByIdAsync(int applicationId, int userId,
            bool isAdminOrOwner)
        {
            var param = new DynamicParameters();

            param.Add("@applicationId", applicationId);
            param.Add("@userId", applicationId);
            param.Add("@isAdminOrOwner", applicationId);

            var result = (await this.QueryAsync<EventApplicationDao>("application_getApplicationById_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result;
        }

        public async Task CreateNewApplicationAsync(EventApplication application, string performerName)
        {
            var param = new DynamicParameters();
            param.Add("@eventId", application.EventId);
            param.Add("@performerId", application.PerformerId);
            param.Add("@performerName", performerName);
            param.Add("@typePerformance", application.TypePerformance.Id);
            param.Add("@durationInMinutes", application.DurationInMinutes);
            param.Add("@currentStatus", application.CurrentStatus.Id);

            var result = await this.ExecuteAsync("application_createNewEventApplication_I", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException(ResponseStrings.CreateNewApplicationFailed);
            }
        }

        public async Task UpdateAsync(EventApplication application)
        {
            var param = new DynamicParameters();
            param.Add("@applicationId", application.Id);
            param.Add("@typePerformance", application.TypePerformance.Id);
            param.Add("@durationInMinutes", application.DurationInMinutes);
            param.Add("@currentStatus", application.CurrentStatus.Id);
            param.Add("@isApplicant", application.LastModifiedByApplicant);

            var result = await this.ExecuteAsync("application_updateEventApplication_U", param,
                this.Transaction, commandType: CommandType.StoredProcedure);

            if (result <= 0)
            {
                throw new DbException(ResponseStrings.ApplicationUpdateFailed);
            }
        }

        public async Task<List<EventApplicationDetailsDao>> GetApplicationsWithDetailsAsync(int eventId,
            int? statusApplication = null)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);
            param.Add("@statusApplication", statusApplication);

            var result =
                await this
                    .QueryAsync<EventApplicationDao, PerformerDao, Tuple<EventApplicationDao, PerformerDao>>(
                        "application_getApplicationsWithDetails_S",
                        Tuple.Create, splitOn: "Id,Id",
                        param: param, this.Transaction, commandType: CommandType.StoredProcedure);

            var responseList = new List<EventApplicationDetailsDao>();

            if (result == null)
            {
                return responseList?.ToList();
            }

            foreach (var (applicationDao, performerDao) in result)
            {
                var itemList = new EventApplicationDetailsDao()
                {
                    PerformerDao = performerDao,
                    EventApplicationDao = applicationDao
                };

                responseList.Add(itemList);
            }


            return responseList?.ToList();
        }
    }
}