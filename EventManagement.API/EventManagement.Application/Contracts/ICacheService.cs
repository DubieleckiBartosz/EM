using System;
using System.Threading.Tasks;

namespace EventManagement.Application.Contracts
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T cacheData, TimeSpan? time = null, TimeSpan? slidingTime = null);
        Task RemoveByKey(string key);
    }
}
