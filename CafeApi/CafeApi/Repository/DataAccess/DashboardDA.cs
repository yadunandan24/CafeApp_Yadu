using CafeApi.DAHelper;
using CafeApi.Models.DataModels;
using CafeApi.Repository.Contracts;

namespace CafeApi.Repository.DataAccess
{
    public class DashboardDA : IDashboard
    {
        private readonly ICommonDAHelper _daHelper;
        public DashboardDA(ICommonDAHelper daHelper)
        {
            _daHelper = daHelper;
        }

        public ResponseModel<dynamic> GetDetails()
        {
            ResponseModel<dynamic> responseModel = null;

            IEnumerable<dynamic> result = _daHelper.QueryProcedure<dynamic>("usp_DashboardCount", null);
            var data = new
            {
                category = result.First().categoryCount,
                product = result.First().productCount,
                bill = result.First().billCount,
                user = result.First().userCount
            };

            responseModel = new ResponseModel<dynamic> { StatusCode = System.Net.HttpStatusCode.OK, Data = data };
            return responseModel;
        }
    }


}
