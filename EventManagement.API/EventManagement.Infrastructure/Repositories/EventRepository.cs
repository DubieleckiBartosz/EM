using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Models.Dao;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;
using EventManagement.Application.Models.Dao.EventDAOs;
using EventManagement.Application.Strings.Responses;
using EventManagement.Domain.Entities;
using EventManagement.Infrastructure.Database;

namespace EventManagement.Infrastructure.Repositories
{
    public class EventRepository : BaseRepository, IEventRepository
    {
        public EventRepository(EventContext context) : base(context)
        {
        }

        public async Task<int> CreateEventAsync(Event eventEntity)
        {
            var address = eventEntity.EventAddress;
            var eventTime = eventEntity.EventTime;

            var param = new DynamicParameters();
            param.Add("@eventName", eventEntity.EventName.Name);
            param.Add("@eventDescription", eventEntity.Description.Description);
            param.Add("@startDate", eventTime.StartDate);
            param.Add("@endDate", eventTime.EndDate);
            param.Add("@recurringEvent", eventEntity.RecurringEvent);
            param.Add("@placeType", eventEntity.PlaceType.Id);
            param.Add("@city", address.City);
            param.Add("@street", address.Street);
            param.Add("@numberStreet", address.NumberStreet);
            param.Add("@postalCode", address.PostalCode);
            param.Add("@eventCategory", eventEntity.Category.Id);
            param.Add("@eventType", eventEntity.EventType.Id);
            param.Add("@newIdentifier", -1, DbType.Int32, ParameterDirection.Output);

            await this.ExecuteAsync("event_createNewEvent_I", param, this.Transaction,
                commandType: CommandType.StoredProcedure);
            var eventId = param.Get<int?>("@newIdentifier");
            if (eventId != null)
            {
                return eventId.Value;
            }

            throw new DbException(ResponseStrings.CreationNewEventFailed);
        }


        public async Task<EventWithImagesDao> GetEventWithImagesAsync(int eventId)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);

            var eventDict = new Dictionary<int, EventWithImagesDao>();
            var eventDao = (await this.QueryAsync<EventWithImagesDao, EventImageDao, EventWithImagesDao>(
                    "event_getEventWithImages_S",
                    (e, i) =>
                    {
                        EventWithImagesDao eventValue;
                        if (!eventDict.TryGetValue(e.Id, out eventValue))
                        {
                            eventValue = e;
                            eventValue.Images = new List<EventImageDao>();
                            eventDict.Add(e.Id, e);
                        }

                        if (i != null)
                        {
                            eventValue.Images.Add(i);
                        }

                        return eventValue;
                    }, splitOn: "Id", param: param, this.Transaction, commandType: CommandType.StoredProcedure))
                .FirstOrDefault();

            if (eventDao == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return eventDao;
        }

        public async Task<EventWithOpinionsDao> GetEventWithOpinionsAsync(int eventId, string sortName, string sortType)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);
            param.Add("@sortName", sortName);
            param.Add("@sortType", sortType);

            var eventDict = new Dictionary<int, EventWithOpinionsDao>();
            var eventDao = (await this.QueryAsync<EventWithOpinionsDao, EventOpinionDao, EventWithOpinionsDao>(
                    "event_getEventWithOpinions_S",
                    (e, o) =>
                    {
                        EventWithOpinionsDao eventValue;
                        if (!eventDict.TryGetValue(e.Id, out eventValue))
                        {
                            eventValue = e;
                            eventValue.Opinions = new List<EventOpinionDao>();
                            eventDict.Add(e.Id, e);
                        }

                        if (o != null)
                        {
                            eventValue.Opinions.Add(o);
                        }

                        return eventValue;
                    }, splitOn: "Id", param: param, this.Transaction, commandType: CommandType.StoredProcedure))
                .FirstOrDefault();

            if (eventDao == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return eventDao;
        }

        public async Task UpdateAsync(Event eventEntity)
        {
            var eventTime = eventEntity.EventTime;

            var param = new DynamicParameters();

            param.Add("@eventId", eventEntity.Id);
            param.Add("@eventDescription", eventEntity.Description.Description);
            param.Add("@startDate", eventTime.StartDate);
            param.Add("@endDate", eventTime.EndDate);
            param.Add("@recurringEvent", eventEntity.RecurringEvent);
            param.Add("@placeType", eventEntity.PlaceType.Id);
            param.Add("@eventCategory", eventEntity.Category.Id);
            param.Add("@eventType", eventEntity.EventType.Id);
            param.Add("@currentStatus", eventEntity.CurrentStatus.Id);

            var result = await this.ExecuteAsync("event_updateEvent_U", param, this.Transaction,
                commandType: CommandType.StoredProcedure);
            if (result <= 0)
            {
                throw new DbException("Event could not be updated.");
            }
        }

        public async Task<EventDetailsDao> GetEventDetailsAsync(int eventId, bool isAdmin, bool isOwner)
        {
            var param = new DynamicParameters();
            param.Add("@eventId", eventId);
            param.Add("@isAdmin", isAdmin);
            param.Add("@isOwner", isOwner);

            var result = (await this.QueryMultipleAsync<EventDetailsDao, EventImageDao, EventOpinionDao>(
                sql: "event_getEventDetails_S", splitOn: "Id,Id,Id", param: param, this.Transaction,
                commandType: CommandType.StoredProcedure))?.ToList();

            if (result == null || !result.Any())
            {
                throw new DbException(ResponseStrings.EventNotFound);
            }

            var eventDict = new Dictionary<int, EventDetailsDao>();
            var eventDetails = new EventDetailsDao();
            foreach (var (value, image, opinion) in result)
            {
                if (!eventDict.TryGetValue(value.Id, out eventDetails))
                {
                    eventDetails = value;
                    eventDetails.Images = new List<EventImageDao>();
                    eventDetails.Opinions = new List<EventOpinionDao>();
                    eventDict.Add(value.Id, value);
                }

                var eventImageDao = image;
                if (eventImageDao != null && !eventDetails.Images.Exists(_ => _.Id == eventImageDao.Id))
                {
                    eventDetails.Images.Add(eventImageDao);
                }

                if (opinion != null && !eventDetails.Opinions.Exists(_ => _.Id == opinion.Id))
                {
                    eventDetails.Opinions.Add(opinion);
                }
            }

            return eventDetails;
        }

        public async Task<EventBaseDao> GetEventBaseDataAsync(int eventId)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);

            var result = (await this.QueryAsync<EventBaseDao>("event_getEventBaseDataById_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (result == null)
            {
                throw new DbException(ResponseStrings.EventNotFound);
            }

            return result;
        }

        public async Task<EventWithApplicationsDao> GetEventWithApplicationsAsync(int eventId, 
            int? statusApplication = null)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);
            param.Add("@statusApplication", statusApplication);

            var eventWithApplicationsDict = new Dictionary<int, EventWithApplicationsDao>();
            var result =
                (await this.QueryAsync<EventWithApplicationsDao, EventApplicationDao, EventWithApplicationsDao>(
                    "event_getEventWithApplications_S", (e, ea) =>
                    {
                        EventWithApplicationsDao eventValue;
                        if (!eventWithApplicationsDict.TryGetValue(e.Id, out eventValue))
                        {
                            eventValue = e;
                            eventValue.EventApplications = new List<EventApplicationDao>();
                            eventWithApplicationsDict.Add(e.Id, e);
                        }

                        if (ea != null)
                        {
                            eventValue.EventApplications.Add(ea);
                        }

                        return eventValue;
                    }, splitOn: "Id,Id", param,
                    this.Transaction, commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (result == null)
            {
                throw new DbException(ResponseStrings.EventNotFound);
            }

            return result;
        }


        public async Task<List<EventsWithCountDao>> SearchEventsAsync(int? eventId, string eventName, DateTime? from,
            DateTime? to, string city, int? placeType, bool? recurringEvent, int? category, int? eventType,
            int? eventCurrentStatus, string sortName, string sortType, int pageNumber, int pageSize)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);
            param.Add("@eventName", eventName);
            param.Add("@from", from);
            param.Add("@to", to);
            param.Add("@city", city);
            param.Add("@placeType", placeType);
            param.Add("@recurringEvent", recurringEvent);
            param.Add("@category", category);
            param.Add("@eventType", eventType);
            param.Add("@sortName", sortName);
            param.Add("@eventCurrentStatus", eventCurrentStatus);
            param.Add("@sortType", sortType);
            param.Add("@pageNumber", pageNumber);
            param.Add("@pageSize", pageSize);

            var result = (await this.QueryAsync<EventsWithCountDao>("event_getEventsBySearch_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure))?.ToList();
            if (result?.Any() == false)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            //Temp
            return result?.Skip((pageNumber - 1) * pageSize)?.Take(pageSize)?.ToList();
        }

        public async Task<List<EventApplicationContactWithStatusDao>>
            GetDataToNotifiedAboutEventStatusChangeAsync(int eventId)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);

            var result = await this.QueryAsync<EventApplicationContactWithStatusDao>(
                "event_getPerformersDataToNotifiedAboutStatusChange_S", param,
                this.Transaction, commandType: CommandType.StoredProcedure);
            return result?.ToList();
        }

        public async Task<EventWithApplicationDetailsDao>
            GetEventWithApplicationDetailsAsync(int eventId, int? statusApplication = null)
        {
            var param = new DynamicParameters();

            param.Add("@eventId", eventId);
            param.Add("@statusApplication", statusApplication);

            var eventWithApplicationsDict = new Dictionary<int, EventWithApplicationDetailsDao>();
            var result =
                (await this
                    .QueryAsync<EventWithApplicationDetailsDao, EventApplicationDetailsDao,
                        EventWithApplicationDetailsDao>(
                        "event_getEventWithApplicationDetails_S", (e, ea) =>
                        {
                            EventWithApplicationDetailsDao eventValue;
                            if (!eventWithApplicationsDict.TryGetValue(e.Id, out eventValue))
                            {
                                eventValue = e;
                                eventValue.ApplicationDetailsList = new List<EventApplicationDetailsDao>();
                                eventWithApplicationsDict.Add(e.Id, e);
                            }

                            if (ea != null)
                            {
                                eventValue.ApplicationDetailsList.Add(ea);
                            }

                            return eventValue;
                        }, splitOn: "Id,Id,Id", param,
                        this.Transaction, commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (result == null)
            {
                throw new DbException(ResponseStrings.DataNotFound);
            }

            return result;
        }
    }
}