using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Interfaces;
using JacksonVeroneze.DataSignerNet.CertificationAuthority.Domain.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddTransient<ICertificateService, CertificateService>();
        }
    }
}
