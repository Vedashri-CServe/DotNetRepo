using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Configuration;
using TMS.Auth;
using TMS.Entity;
using static TimeTrackerExe.IdleDataManager;

namespace TimeTrackerExe
{
    public partial class LoginForm : Form
    {
        #region Private Variable Declaration
        private readonly bool _isValiduser;
        #endregion

        #region Constructors
        /// <summary>
        /// Intial Login form load
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
            var userCredentials = GetUserCredentials(fileType.credentials);
            if(userCredentials != null)
            {
                txtEmailAddress.Text = userCredentials.Username.ToString();
                txtPassword.Text = userCredentials.Password.ToString();
                btnLogin_Click(new object(), new EventArgs());
            }
        }

        /// <summary>
        /// In case of expired token
        /// </summary>
        /// <param name="isValiduser"></param>
        public LoginForm(bool isValiduser)
        {
            _isValiduser = isValiduser;
        }
        #endregion

        #region Private methods
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (string.IsNullOrEmpty(txtEmailAddress.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Email address and password is required");
                return;
            }

            await SetUserToken();

            if (!string.IsNullOrEmpty(CommonExtension.GetUserToken()))
            {
                await SetUserDetails();

                //save username and password in textfile
                var userCred = new UserCredentials
                {
                    Username = txtEmailAddress.Text,
                    Password = txtPassword.Text
                };

                IdleDataManager.SaveDataToFile(userCred, IdleDataManager.fileType.credentials);

                var logoutDetails = IdleDataManager.GetDataFromFile<LogoutUserVM>(DateTime.UtcNow.Date, fileType.shutdown_log);
                if (logoutDetails.UserId != 0)
                    _ = IdleDataManager.SaveLogoutTime(logoutDetails);

                OpenIdleSystemForm();
            }

            Cursor = Cursors.Default;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            //Need to change with check ValidateTokenTimeBased
            if (!string.IsNullOrEmpty(CommonExtension.GetUserToken()))
                OpenIdleSystemForm();
        }

        private void OpenIdleSystemForm()
        {
            Hide();
            var idleForm = new IdleForm();
            idleForm.WindowState = FormWindowState.Minimized;
            idleForm.Show();
        } 
        #endregion

        #region Set & Get Tokens
        public async Task SetUserToken() => Environment.SetEnvironmentVariable(CommonExtension.TokenKey, await GetUserToken(txtEmailAddress.Text, txtPassword.Text));

        public async Task SetUserTokenWithCred(string UserName,string Password) => Environment.SetEnvironmentVariable(CommonExtension.TokenKey, await GetUserToken(UserName, Password));     

        private async Task<string> GetUserToken(string userName, string userPassword)
        {
            var userCredentials = new UserCredentials
            {
                Username = userName,
                Password = userPassword
            };
            var token = string.Empty;

            var response = await CommonExtension.ExcuteAsync<UserCredentials, TokenResponseData>(userCredentials, UrlConstants.Token, RequestType.POST);

            if (response?.ResponseStatus == ResponseStatuses.Success)
            {
                token = response?.ResponseData?.Token?.Token ?? string.Empty;
            }
            else
            {
                MessageBox.Show(response?.Message ?? response?.ErrorData?.Error);
            }
            return token;
        }

        #endregion

        #region Set & Get User Details
        private async Task SetUserDetails() => IdleDataManager.configData = await GetUserDetails();
        private async Task<TimeTrackerConfig> GetUserDetails()
        {
            TimeTrackerConfig config = new();

            var response = await CommonExtension.ExcuteAsync<object, ConfigData>(null, UrlConstants.GetUserDetail, RequestType.GET, CommonExtension.GetUserToken());

            if (response?.ResponseStatus == ResponseStatuses.Success)
            {
                config = new TimeTrackerConfig
                {
                    UserId = response.ResponseData?.UserId ?? default,
                    FirstName = response.ResponseData?.FirstName ?? string.Empty,
                    LastName = response.ResponseData?.LastName ?? string.Empty,
                    RoleName = response.ResponseData?.RoleName[0] ?? string.Empty,
                };
            }
            else
            {
                MessageBox.Show(response?.Message ?? response?.ErrorData?.Error);
            }
            return config;
        }
        #endregion

        #region Validations for Email & Password
        private void txtEmailAddress_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmailAddress.Text.Trim()))
                reqEmailAddress.SetError(txtEmailAddress, "Email address is required.");
            else
                reqEmailAddress.SetError(txtEmailAddress, string.Empty);
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(Convert.ToString(txtPassword.Text.Trim())))
                reqPassword.SetError(txtPassword, "Password is required");
            else
                reqPassword.SetError(txtPassword, string.Empty);
        }
        #endregion

        #region Mouse & keyboard events
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{tab}");
                e.SuppressKeyPress = true;
            }
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = false;
        } 
        #endregion
    }
}
