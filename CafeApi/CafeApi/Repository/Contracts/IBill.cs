using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Repository.Contracts
{
    public interface IBill
    {
        public ResponseModel<string> GenerateReport(string authorization, Billmodel bill);

        public (byte[] fileBytes, string fileName, string contentType) GetPdf(string authorization, Billmodel bill);

        public ResponseModel<IEnumerable<Billmodel>> GetBills(string authorization);

        public ResponseModel<string> DeleteBill(string authorization, int id);
    }
}
