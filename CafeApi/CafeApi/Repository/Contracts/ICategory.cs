using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Repository.Contracts
{
    public interface ICategory
    {
        public ResponseModel<string> AddNewCategory(string authorization, Category category);
        public ResponseModel<IEnumerable<Category>> GetAllCategory();

        public ResponseModel<string> UpdateCategory(string authorization, Category category);
    }
}
