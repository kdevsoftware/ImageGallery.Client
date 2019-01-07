using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentValidation.AspNetCore;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
using Loggly;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;

namespace ImageGallery.Client
{
    /// <summary>
    ///
    /// </summary>
    public class Startup
    {
        /// <summary />
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Log.Information("Startup:ImageGallery.Client");
            Configuration = configuration;
            HostingEnvironment = env;
        }

        /// <summary>
        ///
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
            });

            Log.Information("ConfigureServices:ImageGallery.Client");

            services.AddOptions();
            services.Configure<ApplicationOptions>(Configuration);
            services.Configure<ApplicationOptions>(Configuration.GetSection("applicationSettings"));

            services.DisplayConfiguration(Configuration);
            services.AddCustomDataProtection(Configuration);
            services.AddCustomSwagger(Configuration);

            services.AddCors();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(o =>
                {
                    o.RegisterValidatorsFromAssemblyContaining<Startup>();
                });

            services.AddCustomAuthentication(Configuration);
            services.AddCustomAuthorization(Configuration);

            // Http Clients
            services.AddHttpClientImageGalleryApi(Configuration);
            services.AddHttpClientUserManagementApi(Configuration);
            // TODO: NavigatorIdentityHttpClient

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<ILogglyClient, LogglyClient>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    HotModuleReplacementEndpoint = "/dist/__webpack_hmr",
                });
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false,
            };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardOptions);

            var config = Configuration.Get<ApplicationOptions>();

            app.UseAuthentication();
            app.UseStaticFiles();

            if (config.SwaggerUiConfiguration.Enabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var swaggerApi = "/swagger/v1/swagger.json";
                    options.SwaggerEndpoint(swaggerApi, "ImageGallery.Client V1");
                    options.ConfigureOAuth2(config.SwaggerUiConfiguration.ClientId, string.Empty, string.Empty, "Swagger UI");
                });
            }

            app.UseCors(options => options.AllowAnyMethod());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Gallery", action = "Index" });
            });
        }
    }

    /// <summary>
    /// Service Collection Configuration Methods.
    /// </summary>
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

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.Get<ApplicationOptions>();
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = config.OpenIdConnectConfiguration.Authority;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudiences = new List<string>
                        {
                            $"{config.OpenIdConnectConfiguration.Authority}/resources",
                            config.OpenIdConnectConfiguration.ClientId,
                        },
                    };
                });

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "CanOrderFrame",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("country", "be");
                        policyBuilder.RequireClaim("subscriptionlevel", "PayingUser");
                        policyBuilder.RequireRole("PayingUser");
                    });
            });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.Get<ApplicationOptions>();
            if (config.SwaggerUiConfiguration.Enabled)
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = "ImageGallery.Client",
                        Description = "ImageGallery.Client",
                        Version = "v1",
                    });

                    options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = $"{config.OpenIdConnectConfiguration.Authority}/connect/authorize",
                        TokenUrl = $"{config.OpenIdConnectConfiguration.Authority}/connect/token",
                        Scopes = new Dictionary<string, string>()
                        {
                            { "imagegalleryapi", "Image Gallery API" },
                        },
                    });
                    options.IncludeXmlComments(GetXmlCommentsPath());
                    options.AddFluentValidationRules();
                });
            }

            return services;
        }

        public static IServiceCollection AddCustomDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.Get<ApplicationOptions>();

            Console.WriteLine($"Data Protection Enabled:{config.Dataprotection.Enabled}");
            if (config.Dataprotection.Enabled)
            {
                var redis = ConnectionMultiplexer.Connect(config.Dataprotection.RedisConnection);
                services.AddDataProtection().PersistKeysToRedis(redis, config.Dataprotection.RedisKey);
            }

            return services;
        }

        /* Http Clients */
        public static IServiceCollection AddHttpClientImageGalleryApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("imagegallery-api", async (s, x) =>
                {
                    var accessToken = await s.GetRequiredService<IHttpContextAccessor>().HttpContext
                        .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                    if (!string.IsNullOrWhiteSpace(accessToken))
                        x.SetBearerToken(accessToken);

                    var apiUri = s.GetRequiredService<IOptions<ApplicationOptions>>()?.Value?.ApiUri;
                    x.BaseAddress = new Uri(apiUri);
                    x.DefaultRequestHeaders.Accept.Clear();
                    x.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddTypedClient<ImageGalleryHttpClient>();

            return services;
        }

        public static IServiceCollection AddHttpClientUserManagementApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("user-management", async (s, x) =>
                {
                    var accessToken = await s.GetRequiredService<IHttpContextAccessor>().HttpContext
                        .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                    if (!string.IsNullOrWhiteSpace(accessToken))
                        x.SetBearerToken(accessToken);

                    var apiUri = s.GetRequiredService<IOptions<ApplicationOptions>>()?.Value?.ClientConfiguration?.ApiUserManagementUri;
                    x.BaseAddress = new Uri(apiUri);
                    x.DefaultRequestHeaders.Accept.Clear();
                    x.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddTypedClient<UserManagementHttpClient>();

            return services;
        }

        private static string GetXmlCommentsPath()
        {
            var basePath = AppContext.BaseDirectory;
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var fileName = Path.GetFileName(assemblyName + ".xml");

            return Path.Combine(basePath, fileName);
        }
    }
}
