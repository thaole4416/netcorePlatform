using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Services;

namespace Platform
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        private IConfiguration Configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITimeStamper, DefaultTimeStamper>();
            services.AddScoped<IResponseFormatter, TextResponseFormatter>();
            services.AddScoped<IResponseFormatter, HtmlResponseFormatter>();
            services.AddScoped<IResponseFormatter, GuidService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseMiddleware<WeatherMiddleware>();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/middleware/function")
                {
                    IResponseFormatter formatter = context.RequestServices.GetService<IResponseFormatter>();
                    await formatter.Format(context, "Middleware Function: It is snowing in Chicago");
                }
                else
                {
                    await next();
                }
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/single", async context =>
                {
                    IResponseFormatter formatter = context.RequestServices.GetService<IResponseFormatter>();
                    await formatter.Format(context, "Single service");
                });
                endpoints.MapGet("/", async context =>
                {
                    IResponseFormatter formatter = context.RequestServices.GetServices<IResponseFormatter>()
                        .First(f => f.RichOutput);
                    await formatter.Format(context, "Multiple services");
                });
            });
        }
    }
}