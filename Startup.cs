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
            services.Configure<RouteOptions>(opts =>
            {
                opts.ConstraintMap.Add("countryName", typeof(CountryRouteConstraint));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("{number:int}",
                        async context => { await context.Response.WriteAsync("Routed to the int endpoint"); })
                    .Add(b => ((RouteEndpointBuilder) b).Order = 1);
                ;
                endpoints.Map("{number:double}",
                        async context => { await context.Response.WriteAsync("Routed to the double endpoint"); })
                    .Add(b => ((RouteEndpointBuilder) b).Order = 2);
                ;
            });
            app.Use(async (context, next) => { await context.Response.WriteAsync("Terminal Middleware Reached"); });
        }
    }
}