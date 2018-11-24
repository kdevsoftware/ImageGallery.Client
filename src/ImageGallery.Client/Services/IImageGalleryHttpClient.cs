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
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HttpClient> GetClient();

        /// <summary>
        ///
        /// </summary>
        /// <param name="apiUri"></param>
        /// <returns></returns>
        Task<HttpClient> GetClient(string apiUri);
    }
}
