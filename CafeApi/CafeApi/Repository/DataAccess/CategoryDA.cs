using CafeApi.Common;
using CafeApi.DAHelper;
using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Web.Mvc;

namespace CafeApi.Repository.DataAccess
{
    public class CategoryDA : ICategory
    {
        private readonly ICommonDAHelper _daHelper;
        public CategoryDA(ICommonDAHelper dAHelper) 
        {
            _daHelper = dAHelper;
        } 
        public ResponseModel<string> AddNewCategory(string authorization,Category category)
        {
           ResponseModel<string> responseModel = null;

           TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
           if(tokenClaim.Role != "admin")
           {
                 return responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "You do not have access to add new category"};
           }

            string sql = "Insert into Category values (@name)";
            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
                {
                    new QueryParameterForSqlMapper
                    {
                        Name = "name",
                        Value = category.Name,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    }
            };
            int result = _daHelper.Execute(sql, parameterCollection);

            responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Category added successfully" };
            return responseModel;
        }

        public ResponseModel<IEnumerable<Category>> GetAllCategory()
        {
            ResponseModel<IEnumerable<Category>> responseModel = null;
            IEnumerable<Category> categories = new List<Category>();

            string sql = "Select * from category";
            categories = _daHelper.Query<Category>(sql, null);

            responseModel = new ResponseModel<IEnumerable<Category>> { StatusCode = HttpStatusCode.OK, Data = categories , Message = "categories returned successfully"};
            return responseModel; 
        }

        public ResponseModel<string> UpdateCategory(string authorization,Category category)
        {
            ResponseModel<string> responseModel = null;
            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if (tokenClaim.Role != "admin")
            {
                return responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "You do not have access to update new category" };
            }

            dynamic result = null;
            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
                {
                    new QueryParameterForSqlMapper
                    {
                        Name = "Id",
                        Value = category.Id,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "Name",
                        Value = category.Name,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },

            };
            result = _daHelper.ExecuteScalarProcedure("usp_UpdateCategory", parameterCollection);
            if (result == 1) 
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Category updated successfully"};
            }
            if(result == 0)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Category Id does not exist" };
            }

            return responseModel;
        }
    }
}
