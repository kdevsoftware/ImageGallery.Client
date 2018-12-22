using System.Net.Http;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    /// <summary>
    ///
    /// </summary>
    public interface IImageGalleryHttpClient
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="apiUri"></param>
        /// <returns></returns>
        Task<HttpClient> GetClient(string apiUri);
    }
}
