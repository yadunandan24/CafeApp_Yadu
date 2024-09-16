using CafeApi.Models.DataModels;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboard _dashboard;
        public DashboardController(IDashboard dashboard)
        {
            _dashboard = dashboard;
        }

        [Authorize]
        [HttpGet, Route("details")]
        public ActionResult<ResponseModel<dynamic>> GetDetails()
        {
            try
            {
                return _dashboard.GetDetails();
            }
            catch (Exception ex)
            {
                return new ResponseModel<dynamic> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

    }
}
