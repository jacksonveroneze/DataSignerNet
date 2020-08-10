using DataSignerNet.Domain.Services;
using DataSignerNet.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataSignerNet.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<ICertificateService, CertificateService>();
            services.AddTransient<ISignatureService, SignatureService>();
        }
    }
}