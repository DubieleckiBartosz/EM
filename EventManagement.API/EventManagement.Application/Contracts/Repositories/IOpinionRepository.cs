using System.Threading.Tasks;
using EventManagement.Application.Models.Dao;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Contracts.Repositories
{
    public interface IOpinionRepository
    {
        Task<EventOpinionDao> GetOpinionAsync(int opinionId);
        Task CreateNewOpinionAsync(EventOpinion opinion);
        Task UpdateOpinionAsync(EventOpinion opinion);
        Task RemoveOpinion(int opinionId);
    }
}
