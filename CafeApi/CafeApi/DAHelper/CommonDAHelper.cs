using CafeApi.Common;
using CafeApi.Models;
using CafeApi.Repository.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CafeApi.DAHelper
{
    public class CommonDAHelper : ICommonDAHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<ConnectionString> _options;
        public CommonDAHelper(IConfiguration configuration, IOptions<ConnectionString> options)
        {
            _configuration = configuration;
            _options = options;
        }

        private SqlConnection CreateDatabaseConnection()
        {
            SqlConnection sql = null;
            string dbConnection = _options.Value.dbConn ?? "";
            if(dbConnection !=null && dbConnection != string.Empty)
            {
                sql = new SqlConnection(dbConnection);
                sql.Open();
            }

            return sql;
        }

        private DynamicParameters ConvertToDynamicParameters(IList<QueryParameterForSqlMapper> parameterCollection)
        {
            DynamicParameters dynamicParameters = null;
            if (parameterCollection != null && parameterCollection.Count > 0)
            {
                dynamicParameters = new DynamicParameters();
                foreach (QueryParameterForSqlMapper parameter in parameterCollection)
                {
                    dynamicParameters.Add(parameter.Name, parameter.Value, parameter.DbType, parameter.parameterDirection);
                }
            }
            return dynamicParameters;
        }


        public T QueryFirstorDefault<T>(string sql,IList<QueryParameterForSqlMapper> parameterCollection)
        {
            T result = default;
            if (!string.IsNullOrEmpty(sql))
            {
                DynamicParameters dynamicParameters = ConvertToDynamicParameters(parameterCollection);
                using (var connection = CreateDatabaseConnection())
                {
                    result = connection.QueryFirstOrDefault<T>(sql,dynamicParameters);
                }
            }
            return result;
        }
        public IEnumerable<T> Query<T>(string sql,IList<QueryParameterForSqlMapper> parameterCollection)
        {
            IEnumerable<T> result = null;
            if (!string.IsNullOrEmpty(sql))
            {
                DynamicParameters dynamicParameters = ConvertToDynamicParameters(parameterCollection);
                using (var connection = CreateDatabaseConnection())
                {
                    result = connection.Query<T>(sql,dynamicParameters);
                }
            }
            return result;
        }
   
        public int Execute(string sql,IList<QueryParameterForSqlMapper> parameterCollection)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(sql))
            {
                DynamicParameters dynamicParameters = ConvertToDynamicParameters(parameterCollection);
                using (var connection = CreateDatabaseConnection())
                {
                    result = connection.Execute(sql, dynamicParameters);

                }
            }
            return result;
        }

        public dynamic ExecuteScalarProcedure(string procedure, IList<QueryParameterForSqlMapper> parameterCollection)
        {
            dynamic result = null;
            if (!string.IsNullOrEmpty(procedure))
            {
                DynamicParameters dynamicParameters = ConvertToDynamicParameters(parameterCollection);
                using (var connection = CreateDatabaseConnection())
                {
                    result = connection.ExecuteScalar(procedure, dynamicParameters, commandType: CommandType.StoredProcedure);
                }
            }
            return result;
        }

        public IEnumerable<T> QueryProcedure<T>(string procedure, IList<QueryParameterForSqlMapper> parameterCollection)
        {
            IEnumerable<T> result = null;
            if (!string.IsNullOrEmpty(procedure))
            {
                DynamicParameters dynamicParameters = ConvertToDynamicParameters(parameterCollection);
                using (var connection = CreateDatabaseConnection())
                {
                    result = connection.Query<T>(procedure, parameterCollection,commandType: CommandType.StoredProcedure);
                }
            }
            return result;
        }

    }
}
