using CafeApi.Common;
using CafeApi.DAHelper;
using CafeApi.Repository.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using CafeApi.Models.DataModels;
using CafeApi.Models.TableModels;

namespace CafeApi.Repository.DataAccess
{
    public class UserDA : IUser
    {
        private readonly ICommonDAHelper _daHelper;
        private readonly IOptions<ConnectionString> _options;
        public UserDA(ICommonDAHelper daHelper,IOptions<ConnectionString> options)
        {
            _daHelper = daHelper;
            _options = options;
        }


        public ResponseModel<string> Signup(Usermodel user)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            Usermodel userObj = new Usermodel();

            string sql = ("Select * from Users where email = @email");
            IList<QueryParameterForSqlMapper> parameterCollection1 = new List<QueryParameterForSqlMapper>()
                {
                    new QueryParameterForSqlMapper
                    {
                        Name = "email",
                        Value = user.email,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    }
            };
            userObj = _daHelper.QueryFirstorDefault<Usermodel>(sql,parameterCollection1);

            if (userObj == null)
            {
                user.role = "user";
                user.status = "false";
                sql = "insert into dbo.users values (@name,@contactnumber,@email,@password,@status,@role)";

                IList<QueryParameterForSqlMapper> parameterCollection2 = new List<QueryParameterForSqlMapper>()
                {
                    new QueryParameterForSqlMapper
                    {
                        Name = "name",
                        Value = user.name,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "contactnumber",
                        Value = user.contactNumber,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "email",
                        Value = user.email,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "password",
                        Value = user.password,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "status",
                        Value = user.status,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                    new QueryParameterForSqlMapper
                    {
                        Name = "role",
                        Value = user.role,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },

                };
                var result = _daHelper.Execute(sql, parameterCollection2);

                return new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Successfully registered" };
            }
            else
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.BadRequest, Data = "Email id already exist" }; ;
            }

        }
        public ResponseModel<string> Login(Login loginmodel)
        {
            Usermodel userObj = new Usermodel();


            string sql = "Select * from dbo.Users where email = @email and password = @password";

            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
                {
                    new QueryParameterForSqlMapper
                    {
                        Name = "email",
                        Value = loginmodel.email,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    },
                     new QueryParameterForSqlMapper
                    {
                        Name = "password",
                        Value = loginmodel.password,
                        parameterDirection = ParameterDirection.Input,
                        DbType = DbType.String
                    }
            };

            userObj = _daHelper.QueryFirstorDefault<Usermodel>(sql,parameterCollection);

            if (userObj != null)
            {
                if (userObj.status == "true")
                {
                    string token = TokenManager.GenerateToken(userObj.email, userObj.role);
                    return new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = token };
                }
                else
                {
                    return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "Wait for admin approval" };
                }
            }
            else
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "Incorrect Username or password"};
            }
        }

        public ResponseModel<IEnumerable<Usermodel>> GetAllUser(string authorization)
        {

            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if(tokenClaim.Role != "admin")
            {
                return  new ResponseModel<IEnumerable<Usermodel>> { StatusCode = HttpStatusCode.Unauthorized, Data = null };
            }
            string sql = "select id,name,contactNumber,email,status,role from dbo.users where role = 'user'";
            IEnumerable<Usermodel> userlist = _daHelper.Query<Usermodel>(sql,null);


            return new ResponseModel<IEnumerable<Usermodel>> { StatusCode = HttpStatusCode.OK, Data = userlist };
         
        }

        
       
        public ResponseModel<string> UpdateUserStatus(string authorization, StatusUpdate userstatus)
        {
            
            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);
            if (tokenClaim.Role != "admin")
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = null };
            }

            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                new QueryParameterForSqlMapper
                {
                    Name = "@status",
                    Value = userstatus.status,
                    parameterDirection = ParameterDirection.Input,
                    DbType = DbType.String
                },
                  new QueryParameterForSqlMapper
                {
                    Name = "@id",
                    Value = userstatus.id,
                    parameterDirection = ParameterDirection.Input,
                    DbType = DbType.Int32
                },
            };
            string sql = "select * from dbo.users where id = @id";
           
            Usermodel userobj =_daHelper.QueryFirstorDefault<Usermodel>(sql,parameterCollection);
            if (userobj == null)
            {
                return new ResponseModel<string> { StatusCode = HttpStatusCode.Unauthorized, Data = "Cannot find user id" };
            }

            sql = "Update dbo.users set status = @status where id = @id";
            int result = _daHelper.Execute(sql,parameterCollection);

            return new ResponseModel<string> {StatusCode = HttpStatusCode.OK, Data = "User status updated successfully" };
        }

        public ResponseModel<string> ChangePassword(string authorization, ChangePassword changepassword)
        {
            ResponseModel<string> response = null;

            TokenClaim tokenClaim = TokenManager.ValidateToken(authorization);

            Usermodel userobj = new Usermodel();
            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                new QueryParameterForSqlMapper
                {
                    Name = "@email",
                    Value = tokenClaim.Email,
                    parameterDirection = ParameterDirection.Input,
                    DbType = DbType.String
                },
                  new QueryParameterForSqlMapper
                {
                    Name = "@oldpassword",
                    Value = changepassword.OldPassword,
                    parameterDirection = ParameterDirection.Input,
                    DbType = DbType.String
                },
                    new QueryParameterForSqlMapper
                {
                    Name = "@newpassword",
                    Value = changepassword.NewPassword,
                    parameterDirection = ParameterDirection.Input,
                    DbType = DbType.String
                },
            };

            dynamic result = _daHelper.ExecuteScalarProcedure("usp_ChangePassword", parameterCollection);
            if (result == 1)
            {
               response =  new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Password updated successfully" };
            }
            if (result == 0)
            {
               response = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Incorrect email id or old password" };
            }
            return response;

        }

        public string CreateEmailBody(string email, string password)
        {
                string body = string.Empty;
                using (StreamReader sr = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"StaticFiles\forgot-password.html")))
                {
                    body = sr.ReadToEnd();

                }
                body = body.Replace("{email}", email);
                body = body.Replace("{password}", password);
                body = body.Replace("{frontendUrl}", "http://localhost:4200/");

                return body;
            
        }

        public async Task<ResponseModel<string>> ForgotPassword(Login loginmodel)
        {
            Usermodel userObj = new Usermodel();
            ResponseModel<string> response = null;

            IList<QueryParameterForSqlMapper> parameterCollection = new List<QueryParameterForSqlMapper>()
            {
                new QueryParameterForSqlMapper
                {
                    Name = "@email",
                    Value = loginmodel.email,
                    parameterDirection = ParameterDirection.Input,
                    DbType = DbType.String
                }
            };
            string sql = "select * from Users where email = @email";
            userObj = _daHelper.QueryFirstorDefault<Usermodel>(sql, parameterCollection);


            if (userObj == null)
            {
                response = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Password sent successfully to your email" };
            }

            var message = new MailMessage();
            message.To.Add(new MailAddress(loginmodel.email));
            message.From = new MailAddress(_options.Value.fromEmail);
            message.Subject = "Password by cafe management system";
            message.Body = CreateEmailBody(userObj.email, userObj.password);
            using (var smtp = new SmtpClient(_options.Value.host)
            {
                EnableSsl = true,
                Port = _options.Value.port,
                Credentials = new NetworkCredential(_options.Value.userName, _options.Value.password)
            })
            {
                await smtp.SendMailAsync(message);
                await Task.FromResult(0);
            }

            response = new ResponseModel<string> { StatusCode = HttpStatusCode.OK, Data = "Password sent successfully to your email" };
            return response;
            
        }
    }

   
}
