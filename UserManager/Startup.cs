using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using TMS.Auth;
using TMS.Helper;
using TMS.UserManager.Business;

[assembly: FunctionsStartup(typeof(UserManager.Startup))]
namespace UserManager
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            builder.Services.AddSingleton(_ =>
            {
                return new BlobServiceClient(configuration.GetValue<string>("AZURE_STORAGE_CONNECTION_STRING"));
            });
            builder.Services.AddSingleton<IAccessTokenProvider>(_ => new ValidateJWT(configuration.GetSection("SecretKey").Get<string>()));
            builder.Services.AddTransient<IConfigurationService, ConfigurationService>();
            builder.Services.AddTransient<IAuthService, AuthBo>();
            builder.Services.AddTransient<IUserHelperService, UserHelperService>();
            builder.Services.AddTransient<IUserService, UserBo>();
            builder.Services.AddTransient<IUserSecurityToken, UserSecurityTokenBo>();
            builder.Services.AddTransient<ITimeLogService, TimeLogBo>();
            builder.Services.AddTransient<IUserEmailProvider, UserEmailProvider>();
            builder.Services.AddTransient<IEmailHelper, EmailHelper>();

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = null;
            });

            builder.Services.AddTransient(_ =>
            {
                var connection = new MySqlConnection(configuration.GetConnectionString("MySqlConnectionString"));
                var compiler = new MySqlCompiler();
                return new QueryFactory(connection, compiler,int.MaxValue);
            });

        }
    }
}
