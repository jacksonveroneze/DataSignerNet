using DataSignerNet.Api.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DataSignerNet.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = SerilogExtensions.FactoryLogger();

            Log.Information("Starting up");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
    }
}