using CafeApi.Common;
using CafeApi.DAHelper;
using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace CafeApi.Repository.DataAccess
{
    public class ProductDA: IProduct
    {
        private readonly ICommonDAHelper _daHelper;
        public ProductDA(ICommonDAHelper daHelper)
        {
            _daHelper = daHelper;
        }

        public ResponseModel<string> AddNewProduct(string authorization, Product product)
        {
            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if (tokenClaim.Role != "admin")
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "You do not have access to add new product"};
            }

            product.Status = "true";
            string sql = "insert into dbo.[Products] values (@Name,@CategoryId, @Description, @Price,@Status)";
            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@Name",
                        Value = product.Name,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "@CategoryId",
                        Value = product.CategoryId,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "@Description",
                        Value = product.Description,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "@Price",
                        Value = product.Price,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "@Status",
                        Value = product.Status,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    }
            };

            int result = _daHelper.Execute(sql, parameterCollection);

            return new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Product added successfully"};

        }
        public ResponseModel<IEnumerable<dynamic>> GetAllProduct()
        {
            ResponseModel<IEnumerable<dynamic>> responseModel = null;
            IEnumerable<dynamic> result = null;

            result = _daHelper.QueryProcedure<dynamic>("usp_GetAllProducts", null);

            responseModel = new ResponseModel<IEnumerable<dynamic>> { StatusCode = HttpStatusCode.OK, Data = result };
            return responseModel;
        }

        public ResponseModel<IEnumerable<Product>> GetProductsByCategory(int id)
        {
            ResponseModel<IEnumerable<Product>> responseModel = null;
            IEnumerable<Product> result = new List<Product>();

            string sql = "Select id, name from Products where categoryId = @id and status = @status";
            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@id",
                        Value = id,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "@status",
                        Value = "true",
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
            };

            result = _daHelper.Query<Product>(sql, parameterCollection);

            responseModel = new ResponseModel<IEnumerable<Product>> { StatusCode = HttpStatusCode.OK,Data = result };

            return responseModel;
        }

        public ResponseModel<Product> GetProductById(int id)
        {
            ResponseModel<Product> responseModel = null;
            Product result = null;

            string sql = "Select * from Products where Id = @id";
            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@id",
                        Value = id,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
            };

            result = _daHelper.QueryFirstorDefault<Product>(sql,parameterCollection);
            responseModel = new ResponseModel<Product> { StatusCode = HttpStatusCode.OK, Data = result };

            return responseModel;
        }

        public ResponseModel<string> UpdateProduct(string authorization,Product product)
        {
            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if (tokenClaim.Role != "admin")
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "You do not have access to update product" };
            }
            Product productObj = new Product();
            ResponseModel<string> responseModel = null;

            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@id",
                        Value = product.Id,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "@categoryId",
                        Value = product.CategoryId,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                      new QueryParameterForSqlMapper
                    {
                        Name = "@name",
                        Value = product.Name,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                      new QueryParameterForSqlMapper
                    {
                        Name = "@description",
                        Value = product.Description,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                        new QueryParameterForSqlMapper
                    {
                        Name = "@price",
                        Value = product.Price,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
            };

            int result = _daHelper.ExecuteScalarProcedure("usp_UpdateProduct", parameterCollection);
            if(result == 1)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Product updated successfully" };
            }
            if (result == 0)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Product id does not exist" };
            }
            return responseModel;

        }

        public ResponseModel<string> DeleteProduct(string authorization, int id)
        {
            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if (tokenClaim.Role != "admin")
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "You do not have access to update product" };
            }

            ResponseModel<string> responseModel = null;

            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@id",
                        Value = id,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    }
            };
            int result = _daHelper.ExecuteScalarProcedure("usp_DeleteProduct", parameterCollection);
            if (result == 1)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Product deleted successfully" };
            }
            if (result == 0)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Product id does not exist" };
            }
            return responseModel;
        }

        public ResponseModel<string> UpdateProductStatus( string authorization,Product product)
        {
            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if (tokenClaim.Role != "admin")
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "You do not have access to update product status" };
            }

            ResponseModel<string> responseModel = null;

            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                    new QueryParameterForSqlMapper
                    {
                        Name = "@id",
                        Value = product.Id,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.Int32
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "@status",
                        Value = product.Status,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    }
            };
            int result = _daHelper.ExecuteScalarProcedure("usp_UpdateProductStatus", parameterCollection);
            if (result == 1)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Product status updated successfully" };
            }
            if (result == 0)
            {
                responseModel = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Product id does not exist" };
            }
            return responseModel;

        }

    }
    }

