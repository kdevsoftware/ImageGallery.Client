using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using ImageGallery.Client.Configuration;
using Loggly;
using Loggly.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;
using Serilog.Sinks.SystemConsole.Themes;

namespace ImageGallery.Client
{
    /// <summary>
    ///
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            var config = Configuration.Get<ApplicationOptions>();
            ConfigureLoggly();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)

                 .WriteTo.Logger(lc => lc
                    .Filter.ByExcluding(_ => !config.LoggingConfiguration.ConsoleEnabled))
                    .WriteTo.Console(theme: SystemConsoleTheme.Literate)

                .Enrich.FromLogContext()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.With(new EnvironmentUserNameEnricher())
                .Enrich.With(new MachineNameEnricher())
                .WriteTo.Async(s => s.RollingFileAlternate(
                        LogConfiguration.GetLoggingPath(Configuration),
                        outputTemplate: "[{ProcessId}] {Timestamp} [{ThreadId}] [{Level}] [{SourceContext}] [{Category}] {Message}{NewLine}{Exception}",
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        retainedFileCountLimit: 100,
                        formatProvider: CreateLoggingCulture())
                    .MinimumLevel.Debug())
                .WriteTo.Loggly()
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Debug.Print(msg);
                Debugger.Break();
            });

            try
            {
                Log.Information("Init:ImageGallery.Client");
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .CaptureStartupErrors(true)
                .Build();

        private static void ConfigureLoggly()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var logglyToken = Environment.GetEnvironmentVariable("LOGGLY_TOKEN");
            Console.WriteLine($"LogglyToken:{logglyToken}");

            var config = LogglyConfig.Instance;

            // Trial Loggy Token (Use ENV)
            config.CustomerToken = "23f3beeb-232f-4c71-9c3c-715a1571edb9";
            config.ApplicationName = "ImageGallery.Client";

            config.Transport.EndpointHostname = "logs-01.loggly.com";
            config.Transport.EndpointPort = 443;
            config.Transport.LogTransport = LogTransport.Https;

            config.TagConfig.Tags.AddRange(new ITag[]
            {
                new ApplicationNameTag { Formatter = "application-{0}" },
                new HostnameTag { Formatter = "host-{0}" },
                new SimpleTag { Value = environment },
            });
        }

        private static CultureInfo CreateLoggingCulture()
        {
            var loggingCulture = new CultureInfo($"");

            /*
             with this DateTime and DateTimeOffset string representations will be sortable. By default,
             serialization without a culture or formater will use InvariantCulture. This may or may not be
             desirable, depending on the sorting needs you require or even the region your in. In this sample
             the invariant culture is used as a base, but the DateTime format is changed to a specific representation.
             Instead of the dd/MM/yyyy hh:mm:ss, we'll force yyyy-MM-dd HH:mm:ss.fff which is sortable and obtainable
             by overriding ShortDatePattern and LongTimePattern.

             Do note that they don't include the TimeZone by default, so a datetime will not have the TZ
             while a DateTimeOffset will in it's string representation.
             Both use the longTimePattern for time formatting, but including the time zone in the
             pattern will duplicate the TZ representation when using DateTimeOffset which serilog does
             for the timestamp.

             If you do not require specific formats, this method will not be required. Just pass in null (the default)
             for IFormatProvider in the Loggly() sink configuration method.
            */

            loggingCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            loggingCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss.fff";

            return loggingCulture;
        }
    }
}
