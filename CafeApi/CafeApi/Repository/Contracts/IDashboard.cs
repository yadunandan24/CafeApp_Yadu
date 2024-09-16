using CafeApi.Models.DataModels;

namespace CafeApi.Repository.Contracts
{
    public interface IDashboard
    {
        public ResponseModel<dynamic> GetDetails();
    }
}
