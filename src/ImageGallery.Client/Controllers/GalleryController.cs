using ImageGallery.Client.Configuration;
using ImageGallery.Client.ViewModels.Gallery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client.Controllers
{
    /// <summary>
    /// Gallery View Main Controller.
    /// </summary>
    public class GalleryController : Controller
    {
        private readonly ILogger<GalleryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public GalleryController(IOptions<ApplicationOptions> settings, ILogger<GalleryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Gallery Index Page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            _logger.LogInformation($"Index() of {typeof(GalleryController)}");
            _logger.LogInformation($"ApplicationSettings {ApplicationSettings}");

            var model = new GalleryViewModel
            {
                ApplicationOptions = ApplicationSettings,
            };

            return View(model);
        }
    }
}
