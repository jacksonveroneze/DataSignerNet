using JacksonVeroneze.DataSignerNet.CertificationAuthority.Interfaces;
using JacksonVeroneze.DataSignerNet.CertificationAuthority.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<ICertificateService, CertificateService>();
        }
    }
}
