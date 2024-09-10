using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using Microsoft.Extensions.Options;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using CafeApi.Common;
using CafeApi.DAHelper;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUser _user;

        public UserController(IUser user)
        {
            _user = user;
        }

        [HttpPost, Route("signup")]
        public ActionResult<ResponseModel<string>> Signup([FromBody] Usermodel user)
        {
            try
            {
                return _user.Signup(user);

            }
            catch (Exception ex)
            {

                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };

            }
        }

        [HttpPost, Route("login")]
        public ActionResult<dynamic> Login([FromBody] Login loginmodel)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            try
            {
                return _user.Login(loginmodel);

            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            };

        }


        [Authorize]
        [HttpGet, Route("getAllUser")]
        public ActionResult<ResponseModel<IEnumerable<Usermodel>>> GetAllUsers([FromHeader] string authorization)
        {
            try
            {
                return _user.GetAllUser(authorization);
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<Usermodel>> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }

        }

        [Authorize]
        [HttpPost, Route("updateUserStatus")]
        public ResponseModel<string> UpdateUserStatus([FromHeader] string authorization, StatusUpdate userstatus)
        {
            try
            {
                return _user.UpdateUserStatus(authorization, userstatus);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        [Authorize]
        [HttpPost, Route("changePassword")]
        public ActionResult<ResponseModel<string>> ChangePassword([FromHeader] string authorization,ChangePassword changepassword)
        {
            try
            {
                return _user.ChangePassword(authorization, changepassword);
            }
            catch (Exception ex) 
            {
              return  new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        [HttpPost,Route("forgotPassword")]
        public async Task<ActionResult<ResponseModel<string>>> ForgotPassword([FromBody] Login login)
        {
            try
            {
                return await _user.ForgotPassword(login);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        
    }
}

