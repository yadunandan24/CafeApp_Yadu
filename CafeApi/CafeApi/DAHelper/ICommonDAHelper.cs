using CafeApi.Common;
using CafeApi.Models;

namespace CafeApi.DAHelper
{
    public interface ICommonDAHelper
    {
        public T QueryFirstorDefault<T>(string sql, IList<QueryParameterForSqlMapper> parameterCollection);

        public int Execute(string sql, IList<QueryParameterForSqlMapper> parameterCollection);
        public IEnumerable<T> Query<T>(string sql, IList<QueryParameterForSqlMapper> parameterCollection);

        public dynamic ExecuteScalarProcedure(string procedure, IList<QueryParameterForSqlMapper> parameterCollection);

        public IEnumerable<T> QueryProcedure<T>(string procedure, IList<QueryParameterForSqlMapper> parameterCollection);


    }
}
