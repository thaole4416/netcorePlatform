using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Platform
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(opts => { opts.CheckConsentNeeded = context => true; });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });
            services.AddHsts(opts => {opts.MaxAge = TimeSpan.FromDays(1);opts.IncludeSubDomains = true;});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction()) {app.UseHsts();} 
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseMiddleware<ConsentMiddleware>();
            app.UseSession();
            app.UseRouting();
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync($"HTTPS Request: {context.Request.IsHttps} \n");
                await next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/cookie", async context =>
                {
                    int counter1 = (context.Session.GetInt32("counter1") ?? 0) + 1;
                    int counter2 = (context.Session.GetInt32("counter2") ?? 0) + 1;
                    context.Session.SetInt32("counter1", counter1);
                    context.Session.SetInt32("counter2", counter2);
                    await context.Session.CommitAsync();
                    await context.Response.WriteAsync($"Counter1: {counter1}, Counter2: {counter2}");
                });
                endpoints.MapGet("clear", context =>
                {
                    context.Response.Cookies.Delete("counter1");
                    context.Response.Cookies.Delete("counter2");
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                });
                endpoints.MapFallback(async context => await context.Response.WriteAsync("Hello World!"));
            });
        }
    }
}