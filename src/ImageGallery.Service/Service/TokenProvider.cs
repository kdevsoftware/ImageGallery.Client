using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ImageGallery.Service.Service
{
    public class TokenProvider : ITokenProvider
    {
        public Task<TokenResponse> RequestResourceOwnerPasswordAsync(string userName, string password, string api)
        {
            throw new NotImplementedException();
        }
    }
}
