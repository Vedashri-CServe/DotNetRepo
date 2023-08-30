namespace TMS.Helper
{
    public static class RandomString
    {
        public const int forgortPasswordOTPSize = 4;

        private static readonly Random random = new();

        private static string GenerateRandomSequence(int length, string chars) =>
            new(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

        public static string GenerateNumericString(int length) => GenerateRandomSequence(length, "0123456789");

        public static string GenerateRandomString(int length) => GenerateRandomSequence(length, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()+");
    }
}
