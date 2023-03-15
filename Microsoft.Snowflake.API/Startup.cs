using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsot.Snowflake.Services;
using Snowflake.Data.Client;

[assembly: FunctionsStartup(typeof(Microsoft.Snowflake.API.Startup))]
namespace Microsoft.Snowflake.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddLogging();

            builder.Services.AddScoped<IRepoService, SnowflakeRepo>();

            builder.Services.AddSingleton(Provider => {
                return new SnowflakeDbConnection(config.GetConnectionString("snowflakeConn"));                
            });

        }
    }
}
