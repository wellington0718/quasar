using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SharedKernel.Interfaces
{
    public interface IBaseDataAccess
    {
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string sql, U parameter, CommandType commandType);
        Task<T> QuerySingleAsync<T, U>(string sql, U parameter, CommandType commandType);
        Task<int> SaveDataAsync<T>(string sql, T parameter, CommandType commandType);
    }
}
