using CafeApi.DAHelper;

namespace CafeApi.Common
{
    public static class AppServiceConfigExtensioncs
    {

        public static void AddAllCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.Configure<ConnectionString>(configuration.GetSection("ConnectionStrings"));

            services.AddTransient<ICommonDAHelper, CommonDAHelper>();

        }
    }
}
