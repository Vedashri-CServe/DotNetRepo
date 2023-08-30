namespace TMS.Entity
{
    public class UserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ValidateUserTokenVM
    {
        public string Token { get; set; }
        public int TokenType { get; set; }
    }

    public class SetPasswordVM
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public int TokenType { get; set; }
    }
    public class ForgotPasswordVM
    {
        public string UserEmailId { get; set; }
    }

    public class Logout
    {
        public bool isLogout { get; set; }
    }

    public class LogoutUserVM
    {
        public bool isLogout { get; set; }
        public long UserId { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
}
