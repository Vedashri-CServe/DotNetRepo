using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace TMS.Auth
{
    public class GenerateJWTToken
    {
        private readonly IJwtAlgorithm _algorithm;
        private readonly IJsonSerializer _serializer;
        private readonly IBase64UrlEncoder _base64Encoder;
        private readonly IJwtEncoder _jwtEncoder;
        private readonly string _secretKey;

        public GenerateJWTToken(string secretKey)
        {
            // JWT specific initialization.
            _algorithm = new HMACSHA256Algorithm();
            _serializer = new JsonNetSerializer();
            _base64Encoder = new JwtBase64UrlEncoder();
            _jwtEncoder = new JwtEncoder(_algorithm, _serializer, _base64Encoder);
            _secretKey = secretKey;
        }

        public TokenReponseVM IssuingJWT(string user)
        {
            var tokenExpiry = DateTime.UtcNow.AddHours(8);
            Dictionary<string, object> claims = new Dictionary<string, object> {
                // JSON representation of the user Reference with ID and display name
                {
                    "username",
                    user
                },
                // TODO: Add other claims here as necessary; maybe from a user database
                {
                    "role",
                    "admin"
                },
                {
                    "expiry",
                   tokenExpiry
                }
            };
            string token = _jwtEncoder.Encode(claims, _secretKey); // Put this key in config
            return new TokenReponseVM
            {
                Token = token,
                TokenExpiry = tokenExpiry,
                Username = user
            };
        }
    }
}