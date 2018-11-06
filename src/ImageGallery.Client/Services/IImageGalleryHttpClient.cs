using System.Net.Http;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    public interface IImageGalleryHttpClient
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<HttpClient> GetClient();

        Task<HttpClient> GetClient(string apiUri);
    }
}
