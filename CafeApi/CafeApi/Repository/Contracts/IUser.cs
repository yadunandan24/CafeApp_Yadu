using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Repository.Contracts
{
    public interface IUser
    {
        public ResponseModel<string> Signup(Usermodel user);
        public ResponseModel<string> Login(Login loginmodel);
        public ResponseModel<IEnumerable<Usermodel>> GetAllUser(string authorization);
        public ResponseModel<string> UpdateUserStatus(string authorization, StatusUpdate userstatus);

        public ResponseModel<string> ChangePassword(string authorization,ChangePassword changepassword);

        public Task<ResponseModel<string>> ForgotPassword(Login user);
    }
}
