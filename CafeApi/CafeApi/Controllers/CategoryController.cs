using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public readonly ICategory _category;
        public CategoryController(ICategory category) 
        { 
            _category = category;
        }

        [Authorize]
        [HttpPost, Route("addNewCategory")]
        public ActionResult<ResponseModel<string>> AddNewCategory([FromHeader] string authorization,[FromBody] Category category)
        {
            try
            {
                return _category.AddNewCategory(authorization,category);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        [Authorize]
        [HttpGet, Route("getAllCategory")]
        public ActionResult<ResponseModel<IEnumerable<Category>>> GetAllCategory()
        {
            try
            {
                return _category.GetAllCategory();
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<Category>> { StatusCode = HttpStatusCode.InternalServerError, Data = null ,Message = ex.Message };
            }
        }


        [Authorize]
        [HttpPost, Route("updateCategory")]
        public ActionResult<ResponseModel<string>> UpdateCategory([FromHeader] string authorization,Category category)
        {
            try
            {
                return _category.UpdateCategory(authorization,category);
            }
            catch (Exception ex) 
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

    }
}
