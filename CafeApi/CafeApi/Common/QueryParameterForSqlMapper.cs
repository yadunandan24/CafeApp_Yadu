using System.Data;
using System.Drawing;

namespace CafeApi.Common
{
    public class QueryParameterForSqlMapper
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection parameterDirection { get; set; }
        public DbType DbType { get; set; }
        public Size Size { get; set; }
    }
}
