using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.Model.Models.Albums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageGallery.Service.Service
{
    public class AlbumService : IAlbumService
    {
        private readonly HttpClient _client;

        private const string AlbumsRoute = "api/albums";

        public AlbumService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<List<Album>> GetUserAlbumsAsync(TokenResponse token)
        {
            List<Album> albumList = new List<Album>();

            _client.SetBearerToken(token.AccessToken);

            var response = await _client.GetAsync(AlbumsRoute);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                albumList = JsonConvert.DeserializeObject<List<Album>>(content);
                Console.WriteLine(JArray.Parse(content));
                Console.WriteLine($"AlbumCount:{albumList.Count}");
                return albumList;
            }

            return albumList;
        }

        public Task<List<Album>> GetUserAlbumsAsync(TokenResponse token, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
