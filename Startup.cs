using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Platform
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("files/{filename}.{ext}", async context => {
                    await context.Response.WriteAsync("Request Was Routed\n");
                    foreach (var kvp in context.Request.RouteValues)
                    {
                        await context.Response.WriteAsync($"{kvp.Key}: {kvp.Value}\n");
                    }
                });
                endpoints.MapGet("capital/{country=France}", Capital.Endpoint);
                endpoints.MapGet("something/{city?}", Population.Endpoint)
                    .WithMetadata(new RouteNameMetadata("population"));
                ;
            });
            app.Use(async (context, next) => { await context.Response.WriteAsync("Terminal Middleware Reached"); });
        }
    }
}