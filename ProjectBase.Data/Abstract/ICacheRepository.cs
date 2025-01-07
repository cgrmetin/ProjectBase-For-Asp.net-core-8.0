namespace ProjectBase.Data.Abstract
{
    public interface ICacheRepository
    {
        Task<T> GetAsync<T>(string key, bool useUserPrefix = false);
        Task SetAsync<T>(string key, T value, TimeSpan? duration = null, bool useUserPrefix = false);
        Task RemoveAsync(string key, bool useUserPrefix = false);
        Task<bool> ExistsAsync(string key, bool useUserPrefix = false);
        Task RemoveByPrefixAsync(string prefix);
    }
}
