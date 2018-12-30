using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.ViewModels.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImageGallery.Client.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("api/[controller]")]
    public class DiagnosticsController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiagnosticsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsController"/> class.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public DiagnosticsController(IHostingEnvironment env, IConfiguration configuration, ILogger<DiagnosticsController> logger)
        {
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///  Heartbeat.
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IActionResult Status() => Ok();

        /// <summary>
        ///  Get Server Diagnostics.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServerDiagnostics), 200)]
        [Produces("application/json", Type = typeof(ServerDiagnostics))]
        public IActionResult Get()
        {
            var diagnostics = GetServerDiagnostics();
            diagnostics.ContentRootPath = _env.ContentRootPath;
            diagnostics.WebRootPath = _env.WebRootPath;
            diagnostics.ApplicationName = _env.ApplicationName;
            diagnostics.EnvironmentName = _env.EnvironmentName;

            var ipAddresses = Dns.GetHostAddressesAsync(diagnostics.DnsHostName).Result.Where(x => x.AddressFamily == AddressFamily.InterNetwork).AsEnumerable().Distinct();

            var enumerable = ipAddresses as IPAddress[] ?? ipAddresses.ToArray();
            var ipList = new List<string>(enumerable.Count());
            foreach (var ipAddress in enumerable)
            {
                ipList.Add(ipAddress.ToString());
            }

            diagnostics.IpAddressList = ipList;

            return Ok(diagnostics);
        }

        private static ServerDiagnostics GetServerDiagnostics()
        {
            var osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;

            var diagnostics = new ServerDiagnostics
            {
                MachineDate = DateTime.Now,
                MachineName = Environment.MachineName,
                MachineCulture =
                    string.Format("{0} - {1}", CultureInfo.CurrentCulture.DisplayName, CultureInfo.CurrentCulture.Name),
                Platform = osNameAndVersion.Trim(),
                DnsHostName = Dns.GetHostName(),
                WorkingDirectory = null,
                Runtime = GetNetCoreVersion(),
            };

            diagnostics.MachineTimeZone = TimeZoneInfo.Local.IsDaylightSavingTime(diagnostics.MachineDate) ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;
            diagnostics.ApplicationVersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            return diagnostics;
        }

        private static string GetNetCoreVersion()
        {
            var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                return assemblyPath[netCoreAppIndex + 1];
            return null;
        }
    }
}
