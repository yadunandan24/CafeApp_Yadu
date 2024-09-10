using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace CafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _product;
        public ProductController(IProduct product)
        {
            _product = product;
        }

        [Authorize]
        [HttpPost, Route("addNewProduct")]
        public ActionResult<ResponseModel<string>> AddNewProduct([FromHeader] string authorization, [FromBody] Product product)
        {
            try
            {
                return _product.AddNewProduct(authorization, product);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }

        }


        [Authorize]
        [HttpGet, Route("getAllProduct")]
        public ActionResult<ResponseModel<IEnumerable<dynamic>>> GetAllProduct()
        {
            try
            {
                return _product.GetAllProduct();
            }
            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<dynamic>> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }

        }

        [Authorize]
        [HttpGet, Route("getProductByCategory/{id}")]
        public ActionResult<ResponseModel<IEnumerable<Product>>> GetProductsByCategory(int id)
        {
            try
            {
                return _product.GetProductsByCategory(id);
            }

            catch (Exception ex)
            {
                return new ResponseModel<IEnumerable<Product>> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }

        }

        [Authorize]
        [HttpGet, Route("getProductById/{id}")]
        public ActionResult<ResponseModel<Product>> GetProductById(int id)
        {
            try
            {
                return _product.GetProductById(id);
            }

            catch (Exception ex)
            {
                return new ResponseModel<Product> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }

        }

        [Authorize]
        [HttpPost, Route("updateProduct")]
        public ActionResult<ResponseModel<string>> UpdateProduct([FromHeader] string authorization, [FromBody] Product product)
        {
            try
            {
                return _product.UpdateProduct(authorization, product);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        [Authorize]
        [HttpPost, Route("deleteProduct/{id}")]
        public ActionResult<ResponseModel<string>> DeleteProduct([FromHeader] string authorization,int id)
        {
            try
            {
                return _product.DeleteProduct(authorization, id);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        [Authorize]
        [HttpPost, Route("updateProductStatus")]
        public ActionResult<ResponseModel<string>>UpdateProductStatus([FromHeader] string authorization, [FromBody] Product product)
        {
            try
            {
                return _product.UpdateProductStatus(authorization,product);
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }
    }
}
