using System;
using ImageGallery.Client.Configuration;
using Loggly;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.Client.Test.Fixtures
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration, IHostingEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ApplicationOptions>(Configuration);
            services.Configure<ApplicationOptions>(Configuration.GetSection("applicationSettings"));

            services.AddSingleton<ILogglyClient, LogglyClient>();

            services.DisplayConfiguration(Configuration);
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseMvc();
        }
    }

    internal static class ServiceCollectionExtensions
    {
        public static void DisplayConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.Get<ApplicationOptions>();

            Console.WriteLine($"Dataprotection Enabled: {config.Dataprotection.Enabled}");
            Console.WriteLine($"Dataprotection Redis: {config.Dataprotection.RedisConnection}");
            Console.WriteLine($"RedisKey: {config.Dataprotection.RedisKey}");

            Console.WriteLine($"ApiAttractionsUri: {config.ClientConfiguration.ApiAttractionsUri}");
            Console.WriteLine($"ApiUserManagementUri: {config.ClientConfiguration.ApiUserManagementUri}");

            Console.WriteLine($"Authority: {config.OpenIdConnectConfiguration.Authority}");
            Console.WriteLine($"ClientId: {config.OpenIdConnectConfiguration.ClientId}");
            Console.WriteLine($"ClientSecret: {config.OpenIdConnectConfiguration.ClientSecret}");
            Console.WriteLine($"LogglyKey: {config.LogglyClientConfiguration.LogglyKey}");
        }
    }
}
