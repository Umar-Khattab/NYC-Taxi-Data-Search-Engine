using System.Collections.Generic;
namespace NycTaxiSearch.Services
{
    public interface ICacheService<T> where T : class
    {
        Task GetAsync(string key);
        Task SetAsync(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task ClearAsync(string pattern);
    }
}