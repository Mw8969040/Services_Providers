using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartPlatform.Application.Common.Interfaces;

namespace SmartPlatform.Infrastructure.Data
{
    public class ReadDbConnection : IReadDbConnection, IDisposable
    {
        private readonly IDbConnection _connection;

        public ReadDbConnection(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            return await _connection.QueryAsync<T>(sql, param);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        {
            return await _connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object? param = null)
        {
            return await _connection.QuerySingleAsync<T>(sql, param);
        }

        public async Task<IMultipleResults> QueryMultipleAsync(string sql, object? param = null)
        {
            var gridReader = await _connection.QueryMultipleAsync(sql, param);
            return new MultipleResults(gridReader);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        private class MultipleResults : IMultipleResults
        {
            private readonly SqlMapper.GridReader _gridReader;

            public MultipleResults(SqlMapper.GridReader gridReader)
            {
                _gridReader = gridReader;
            }

            public async Task<IEnumerable<T>> ReadAsync<T>()
            {
                return await _gridReader.ReadAsync<T>();
            }

            public async Task<T?> ReadFirstOrDefaultAsync<T>()
            {
                return await _gridReader.ReadFirstOrDefaultAsync<T>();
            }

            public async Task<T> ReadSingleAsync<T>()
            {
                return await _gridReader.ReadSingleAsync<T>();
            }

            public void Dispose()
            {
                _gridReader.Dispose();
            }
        }
    }
}
