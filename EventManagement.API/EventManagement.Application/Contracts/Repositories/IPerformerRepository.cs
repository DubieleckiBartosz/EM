using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagement.Application.Models.Dao.PerformerDAOs;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Contracts.Repositories
{
    public interface IPerformerRepository
    {
        Task<List<PerformerDao>> GetAllPerformersAsync();
        Task<PerformerDao> GetPerformerByIdAsync(int userId);
        Task<PerformerWithProposalsDao> GetPerformerWithProposalsAsync(int performerId);
        Task<List<PerformerDao>> GetPerformersByEventIdAsync(int eventId);
        Task CreatePerformerAsync(Performer performer); 
        Task UpdatePerformerAsync(Performer performer);
        Task<List<PerformerWithNumberOfPerformancesDao>> GetPerformersBySearchAsync(int pageNumber,
            int pageSize, string sortName, string sortType, string performerName, bool? vip);
    }
}
