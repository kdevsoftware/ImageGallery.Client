﻿using System;
using System.Collections.Generic;

namespace ImageGallery.Client.ViewModels.Diagnostics
{
    /// <summary>
    /// Server Diagnostics Model.
    /// </summary>
    public class ServerDiagnostics
    {
        /// <summary>
        ///
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ApplicationVersionNumber { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ContentRootPath { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string WebRootPath { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string DnsHostName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime MachineDate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string MachineCulture { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string MachineTimeZone { get; set; }

        /// <summary>
        /// .NET Core Runtime.
        /// </summary>
        public string Runtime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<string> IpAddressList { get; set; }
    }
}
