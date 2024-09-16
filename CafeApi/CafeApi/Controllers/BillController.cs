using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBill _bill;
        public BillController(IBill bill)
        {
            _bill = bill;
        }

        [Authorize]
        [HttpPost, Route("generateReport")]
        public ActionResult<ResponseModel<string>> GenerateReport([FromHeader] string authorization, [FromBody] Billmodel bill)
        {
            try
            {
                return _bill.GenerateReport(authorization, bill);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        [Authorize]
        [HttpPost, Route("getPdf")]
        public ActionResult<dynamic> GetPdf([FromHeader] string authorization, [FromBody] Billmodel bill)
        {
            try
            {
                var (fileBytes, fileName, contentType) = _bill.GetPdf(authorization, bill);
                Response.Headers.Add("Content-Disposition", $"attachment; filename = {fileName}");
                Response.Headers.Add("Content-Length", fileBytes.Length.ToString());

                //File(fileBytes,contentType);

                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                //return new ResponseModel<dynamic> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        [Authorize]
        [HttpGet, Route("getBills")]
        public ActionResult<ResponseModel<IEnumerable<Billmodel>>> GetBills([FromHeader] string authorization)
        {
            try
            {
                return _bill.GetBills(authorization);
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<Billmodel>> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }


        [Authorize]
        [HttpPost, Route("deleteBill/{id}")]
        public ActionResult<ResponseModel<string>> DeleteBill([FromHeader] string authorization, int id)
        {
            try
            {
                return _bill.DeleteBill(authorization, id);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }
    }
}
