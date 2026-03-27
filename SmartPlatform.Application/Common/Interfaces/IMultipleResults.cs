using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartPlatform.Application.Common.Interfaces
{
    public interface IMultipleResults : IDisposable
    {
        Task<IEnumerable<T>> ReadAsync<T>();
        Task<T?> ReadFirstOrDefaultAsync<T>();
        Task<T> ReadSingleAsync<T>();
    }
}
