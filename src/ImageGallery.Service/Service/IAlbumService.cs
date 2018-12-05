using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.Model.Models.Albums;

namespace ImageGallery.Service.Service
{
    public interface IAlbumService
    {
        Task<List<Album>> GetUserAlbumsAsync(TokenResponse token);

        Task<List<Album>> GetUserAlbumsAsync(TokenResponse token, CancellationToken cancellation);
    }
}
