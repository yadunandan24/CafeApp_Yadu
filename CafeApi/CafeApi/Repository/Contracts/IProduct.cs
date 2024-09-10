using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Repository.Contracts
{
    public interface IProduct
    {
        public ResponseModel<string> AddNewProduct(string authorization,Product product);

        public ResponseModel<IEnumerable<dynamic>> GetAllProduct();

        public ResponseModel<IEnumerable<Product>> GetProductsByCategory(int id);
        public ResponseModel<Product> GetProductById(int id);

        public ResponseModel<string> UpdateProduct(string authorization, Product product);
        public ResponseModel<string> DeleteProduct(string authorization,int id);

        public ResponseModel<string> UpdateProductStatus(string authorization,Product product);
    }
}
