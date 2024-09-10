using CafeApi.DAHelper;
using CafeApi.Repository.Contracts;

namespace CafeApi.Repository.DataAccess
{
    public class BillDA:IBill
    {
        private readonly ICommonDAHelper _dahelper;
        public BillDA(ICommonDAHelper dahelper)
        {
            _dahelper = dahelper;
        }
    }
}
