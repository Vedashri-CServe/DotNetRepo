using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
namespace TMS.Auth
{
    public class ValidateJWT : IAccessTokenProvider
    {
        private readonly string _secretKey;

        public ValidateJWT(string secretKey)
        {
            _secretKey = secretKey;
        }

        public TokenValidationVM ValidateToken(HttpRequest request)
        {
            try
            {
                var result = new TokenValidationVM();
                // Check if we have a header.
                if (!request.Headers.ContainsKey("Authorization"))
                {
                    result.IsValid = false;
                    return result;
                }
                string authorizationHeader = request.Headers["Authorization"];
                // Check if the value is empty.
                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    result.IsValid = false;
                    return result;
                }
                // Check if we can decode the header.
                IDictionary<string, object> claims = null;
                try
                {
                    if (authorizationHeader.StartsWith("bearer"))
                    {
                        authorizationHeader = authorizationHeader.Substring(7);
                    }
                    // Validate the token and decode the claims.
                    claims = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm()).WithSecret(_secretKey).MustVerifySignature().Decode<IDictionary<string, object>>(authorizationHeader);
                }
                catch (Exception exception)
                {
                    result.IsValid = false;
                    return result;
                }
                // Check if we have user claim.
                if (!claims.ContainsKey("username"))
                {
                    result.IsValid = false;
                    return result;
                }
                // for token with expiry 30 days
                if (Convert.ToDateTime(claims["expiry"]).Subtract(DateTime.UtcNow).TotalHours > 8)
                {
                    result.IsValid = false;
                    return result;
                }
                if (Convert.ToDateTime(claims["expiry"]) >= DateTime.UtcNow)
                {
                    result.IsValid = true;
                    result.Username = Convert.ToString(claims["username"]);
                    result.Role = Convert.ToString(claims["role"]);
                    return result;
                }
                else
                {
                    result.IsValid = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new TokenValidationVM
                {
                    IsValid = false
                };
            }
        }
    }
}
