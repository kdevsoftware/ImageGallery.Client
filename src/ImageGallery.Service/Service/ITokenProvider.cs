using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ImageGallery.Service.Service
{
    public interface ITokenProvider
    {
        Task<TokenResponse> RequestResourceOwnerPasswordAsync(string userName, string password, string api);
    }
}
