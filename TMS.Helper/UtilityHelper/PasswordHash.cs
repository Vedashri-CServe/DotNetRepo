using CryptSharp.Core;

namespace TMS.Helper
{
    public static class PasswordHash
    {
        public static bool IsPasswordMatchWithHash(string password, string hash)
        {
            try
            {
                return Crypter.CheckPassword(password, hash);
            }
            catch
            {
                return false;
            }
        }
        public static string GenratePasswordHash(string password)
        {
            try
            {
                return Crypter.Blowfish.Crypt(password);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
