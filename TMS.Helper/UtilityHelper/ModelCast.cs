using Newtonsoft.Json;

namespace TMS.Helper
{
    public static class ModelCast
    {
        public static async Task<T> Request<T>(Stream body)
        {
            try
            {
                string requestBody = await new StreamReader(body).ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(requestBody);
            }
            catch (Exception ex)
            {
                throw;
            }   
        }
    }
}
