using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization.Json;
using TMS.Auth;
using TMS.Entity;

namespace TimeTrackerExe
{
    public static class CommonExtension
    {
        #region Static common Methods
        
        #region Variable Declarations
        public static readonly int IdleTime = Convert.ToInt32(ConfigurationManager.AppSettings["IDLETIME"]);

        public static readonly int IdleMaxTime = Convert.ToInt32(ConfigurationManager.AppSettings["IDLEMAXTIME"]);

        public static readonly string TokenKey = "Token";
        #endregion

        public static string GetUserToken() => Environment.GetEnvironmentVariable(TokenKey) ?? string.Empty;

        public static string ToRequest<T>(this T requestModel)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using var memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, requestModel);
            memoryStream.Position = 0;
            using var sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }

        public static T? ToResponse<T>(this string? jsonString) => JsonConvert.DeserializeObject<T?>(jsonString ?? string.Empty);

        /// <summary>
        /// Generic HTTP request function for forms
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="requestModel"></param>
        /// <param name="requestURL"></param>
        /// <param name="requestType"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<TypedApiResponse<R>> ExcuteAsync<T, R>(T? requestModel, string? requestURL, RequestType requestType, string? authorizationToken = null, Dictionary<string, string>? headers = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(authorizationToken)))
                    headers = new Dictionary<string, string>()
                    {
                        {
                            "Authorization", authorizationToken ?? string.Empty
                        }
                    };

                var request = requestModel.ToRequest();
                var response = await new APIRequester().ExecuteRequest(new TMSHttpRequest()
                {
                    Url = requestURL,
                    RequestType = requestType.ToString(),
                    Body = request,
                    Headers = headers
                });

                var apiResponse = response.Body.ToResponse<TypedApiResponse<R>>();
                if (apiResponse != null && apiResponse.ResponseData != null)
                {
                    string jsonString = JsonConvert.SerializeObject(apiResponse.ResponseData);
                    apiResponse.ResponseData = jsonString.ToResponse<R>();
                }

                return apiResponse ?? new TypedApiResponse<R>();
            }
            catch
            {
                throw;
            };
        }



        /// <summary>
        /// Validate token time frequently
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool ValidateTokenTimeBased(string token)
        {
            var TokenInfo = new Dictionary<string, string>();
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var claims = jwtSecurityToken.Claims.ToList();
            string expiryTime;
            foreach (var claim in claims)
            {
                TokenInfo.Add(claim.Type, claim.Value);
            }
            if (TokenInfo.ContainsKey("expiry") && TokenInfo.TryGetValue("expiry", out expiryTime))
            {
                // for token with expiry 30 days
                var difference = Convert.ToDateTime(expiryTime)!.ToUniversalTime().Subtract(DateTime.UtcNow);
                if (difference!.TotalHours > 8)
                {
                    return false;
                }
                if (Convert.ToDateTime(expiryTime)!.ToUniversalTime() >= DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

    public interface IOffsetStopwatch
    {
        public void Start();
        public void Stop();
        public void Reset();
        public void Restart();
        public TimeSpan Elapsed { get; set; }
        public bool IsRunning { get; }        
        public long ElapsedMilliseconds { get; }
        public long ElapsedTicks { get; }
    }

    public class OffsetStopwatch : IOffsetStopwatch
    {
        #region Private variable Declarations
        private Stopwatch _stopwatch;
        TimeSpan _offsetTimeSpan;
        #endregion

        #region Constructors
        public OffsetStopwatch()
        {
            _stopwatch = new Stopwatch();
            _offsetTimeSpan = TimeSpan.Zero;
        }

        public OffsetStopwatch(TimeSpan offsetElapsedTimeSpan)
        {
            _offsetTimeSpan = offsetElapsedTimeSpan;
            _stopwatch = new Stopwatch();
        }
        #endregion

        #region Public methods
        public void Start() => _stopwatch.Start();

        public void Stop() => _stopwatch.Stop();

        public void Reset() => _stopwatch.Reset();

        public void Restart() => _stopwatch.Restart();

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds + _offsetTimeSpan.Milliseconds;

        public long ElapsedTicks => _stopwatch.ElapsedTicks + _offsetTimeSpan.Ticks;

        public bool IsRunning => _stopwatch.IsRunning;

        public TimeSpan Elapsed
        {
            get => _stopwatch.Elapsed + _offsetTimeSpan;
            set => _offsetTimeSpan = value;
        } 
        #endregion
    }
}
