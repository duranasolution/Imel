using ImelMVC.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ImelMVC.Services
{
    public class Methods
    {
        private readonly IConfiguration _config;    
        public Methods(IConfiguration config)
        {
            _config = config;
        }

        public string MakeStringFromToken(string token)
        {
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(token);
            var tokenString = tokenResponse?.Token;
            return tokenString;
        }
    }
}
