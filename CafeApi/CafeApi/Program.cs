using CafeApi.Common;
using CafeApi.DAHelper;
using CafeApi.Repository.Contracts;
using CafeApi.Repository.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace CafeApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetSection("Jwt").GetSection("Issuer").Value,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt").GetSection("SecretKey").Value))
                    };
                });

            builder.Services.AddAllCommonServices(configuration);
            TokenManager.Secret = builder.Configuration.GetSection("Jwt").GetSection("SecretKey").Value;
            TokenManager.Issuer = builder.Configuration.GetSection("Jwt").GetSection("Issuer").Value;

            //builder.Services.AddTransient<ICommonDAHelper, CommonDAHelper>();
            builder.Services.AddTransient<IUser, UserDA>();
            builder.Services.AddTransient<ICategory, CategoryDA>();
            builder.Services.AddTransient<IProduct,ProductDA>();
            builder.Services.AddTransient<IBill, BillDA>();
            builder.Services.AddTransient<IDashboard, DashboardDA>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
