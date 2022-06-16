using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Contracts.Repositories
{
    public interface IEventApplicationRepository
    {
        Task<EventApplicationDao> GetApplicationByIdAsync(int applicationId, int userId,
            bool isAdminOrOwner);

        Task<List<EventApplicationDao>> GetEventApplicationsBySearchAsync(bool hasSuperAccess, int userId, string sortName, string sortType, 
            int? status = null, string eventName = null, DateTime? from = null, DateTime? to = null, int? performanceType = null,
            int? durationInMinutesMin = null, int? durationInMinutesMax = null, bool? lastModifiedByApplicant = null);
        Task CreateNewApplicationAsync(EventApplication application, string performerName);
        Task UpdateAsync(EventApplication application);
        Task<List<EventApplicationDetailsDao>> GetApplicationsWithDetailsAsync(int eventId,
            int? statusApplication = null);
    }
}