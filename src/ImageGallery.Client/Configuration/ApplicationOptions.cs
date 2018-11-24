namespace ImageGallery.Client.Configuration
{
    /// <summary>
    /// Application Configuration Options.
    /// </summary>
    public class ApplicationOptions
    {
        /// <summary>
        /// ImageGallery API Uri.
        /// </summary>
        public string ApiUri { get; set; }

        /// <summary>
        /// Image Base Url.
        /// </summary>
        public string ImagesUri { get; set; }

        /// <summary>
        ///  ClientConfiguration
        /// </summary>
        public ClientConfiguration ClientConfiguration { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Dataprotection Dataprotection { get; set; }

        /// <summary>
        ///
        /// </summary>
        public OpenIdConnectConfiguration OpenIdConnectConfiguration { get; set; }

        /// <summary>
        ///
        /// </summary>
        public LogglyClientConfiguration LogglyClientConfiguration { get; set; }

        /// <summary>
        ///
        /// </summary>
        public LoggingConfiguration LoggingConfiguration { get; set; }

        /// <summary>
        ///
        /// </summary>
        public SwaggerUiConfiguration SwaggerUiConfiguration { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ApiUri:{ApiUri}|ImagesUri{ImagesUri}";
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class Dataprotection
    {
        /// <summary>
        ///
        /// </summary>
        public string RedisConnection { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string RedisKey { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class OpenIdConnectConfiguration
    {
        /// <summary>
        ///
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ResponseType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }
    }

    /// <summary>
    /// Swagger Ui OAuth Configuration.
    /// </summary>
    public class SwaggerUiConfiguration
    {
        /// <summary>
        ///
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        ///
        /// </summary>
        public string RollingFilePath { get; set; }

        /// <summary>
        ///  Serilog Console Logging.
        /// </summary>
        public bool ConsoleEnabled { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class LogglyClientConfiguration
    {
        /// <summary>
        /// Loggly Configuration Key.
        /// </summary>
        public string LogglyKey { get; set; }
    }
}
