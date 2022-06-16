using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;
using EventManagement.Application.Models.Dao.EventDAOs;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Contracts.Repositories
{
    public interface IEventRepository
    {
        Task<int> CreateEventAsync(Event eventEntity);
        Task UpdateAsync(Event eventEntity);
        Task<EventWithImagesDao> GetEventWithImagesAsync(int eventId);
        Task<EventWithOpinionsDao> GetEventWithOpinionsAsync(int eventId, string sortName = null, string sortType = null);
        Task<EventDetailsDao> GetEventDetailsAsync(int eventId, bool isAdmin, bool isOwner);
        Task<List<EventsWithCountDao>> SearchEventsAsync(int? eventId, string eventName, DateTime? from,
            DateTime? to, string city, int? placeType, bool? recurringEvent, int? category, int? eventType,
            int? eventCurrentStatus, string sortName, string sortType, int pageNumber, int pageSize);
        Task<EventBaseDao> GetEventBaseDataAsync(int eventId);
        Task<EventWithApplicationsDao> GetEventWithApplicationsAsync(int eventId, int? statusApplication = null);
        Task<List<EventApplicationContactWithStatusDao>> GetDataToNotifiedAboutEventStatusChangeAsync(int eventId);
    }
}