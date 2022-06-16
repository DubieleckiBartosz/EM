using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EventManagement.Application.Models.Dao;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Contracts.Repositories
{
    public interface IEventImageRepository
    {
        Task<List<EventImageDao>> GetImagesByEventIdAsync(int eventId);
        Task AddNewImageAsync(EventImage eventImage, int eventId, IDbTransaction transaction = null);
        Task RemoveImageAsync(int imageId);
        Task UpdateMainStatus(int imageId, bool isMain);
        Task<EventImageDao> GetImageByIdAsync(int imageId);
    }
}
