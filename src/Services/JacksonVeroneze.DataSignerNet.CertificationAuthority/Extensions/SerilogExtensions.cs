using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Extensions
{
    public class SerilogExtensions
    {
        public static ILogger FactoryLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .WriteTo.LiterateConsole()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Literate)
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
