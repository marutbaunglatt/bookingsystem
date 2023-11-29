namespace BookingSystem1.Services
{
    public interface IRedisCache
    {
        Task<T> GetData<T>(string key);
        Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime);
        Task<object> RemoveData(string key);
    }
}
