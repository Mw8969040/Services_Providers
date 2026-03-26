namespace SmartPlatform.Application.Common.Interfaces
{
    public interface IReadDbConnection
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
        Task<T> QuerySingleAsync<T>(string sql, object? param = null);
    }
}
