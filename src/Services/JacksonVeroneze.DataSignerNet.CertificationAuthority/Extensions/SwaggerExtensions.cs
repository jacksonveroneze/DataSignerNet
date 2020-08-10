using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace JacksonVeroneze.DataSignerNet.CertificationAuthority.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Data Signer - NET",
                    Description = "Data Signer - NET.",
                    Contact = new OpenApiContact() {Name = "Jackson Veroneze", Email = "jackson@jacksonveroneze.com"},
                    License = new OpenApiLicense() {Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT")}
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

            return app;
        }
    }
}
