using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMS.Auth;
using TMS.Entity;
using static TimeTrackerExe.IdleDataManager;

namespace TimeTrackerExe
{
    public partial class IdleForm : Form
    {
        #region declare private variables 

        private static bool isIdle = false;
        private const int NotifyForThisSession = 0; // This session only
        private const int SessionChangeMessage = 0x02B1;
        private const int SessionLockParam = 0x7;
        private const int SessionUnlockParam = 0x8;
        private const int WM_QUERYENDSESSION = 0x11;
        private static bool systemShutdown = false;

        private TimeSpan idleTimeSpan = new TimeSpan();
        #endregion

        #region Import DLLs
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(out LASTINPUTINFO plii);
        [DllImport("wtsapi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd,
        int dwFlags);

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr
        hWnd);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }
        #endregion

        public IdleForm()
        {
            InitializeComponent();
        }

        private void IdleForm_Load(object sender, EventArgs e)
        {
            WTSRegisterSessionNotification(this.Handle, NotifyForThisSession);
            UserName.Text = string.Concat(IdleDataManager.configData.FirstName, " ", IdleDataManager.configData.LastName);
            UserName.Visible = true;
            notifyIcon.Visible = false;
        }

        private async void idleTimer_Tick(object sender, EventArgs e)
        {
           
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)LASTINPUTINFO.SizeOf;
            GetLastInputInfo(out lastInputInfo);

            int elapsedTicks = Environment.TickCount - (int)lastInputInfo.dwTime;
            var res = new TimeSpan(0, 0, 0, 0, elapsedTicks);
            idleTimeSpan = idleTimeSpan.Add(res);
            var idleTime = elapsedTicks / 60000;
            var insertExitTime = true;

            if (idleTime >= CommonExtension.IdleTime && isIdle == false)
            {
                IdleDataManager.ErrorToFile($"calling idleTimer_tick after system is idle function", fileType.system_log);
                isIdle = true;

                var startTimeJsonObj = new IdleTimeVM
                {
                    UserId = IdleDataManager.configData.UserId,
                    StartTime = DateTime.UtcNow.AddMinutes(-(CommonExtension.IdleTime)),
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow,
                    EventType = (int)TaskLogEventType.IdleStart
                };

                IdleDataManager.SaveDataToFile(startTimeJsonObj, IdleDataManager.fileType.data);

                //save log for pause tasks
                OnSessionLock(true, true);

                MessageBox.Show("TMS is idle, please click on the \"OK\" button to start again.", "TMS", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                if (!CommonExtension.ValidateTokenTimeBased(CommonExtension.GetUserToken().ToString()))
                {
                    LoginForm _login = new LoginForm(false);
                    var UserCred = GetUserCredentials(fileType.credentials);
                    if (UserCred != null)
                    {
                        await _login.SetUserTokenWithCred(UserName: UserCred!.Username.ToString(), Password: UserCred!.Password.ToString());
                    }
                    insertExitTime = false;
                }

                if (insertExitTime)
                {
                    //Idle StartTime entry in Table
                    var StartTimeResponse = await IdleDataManager.SaveIdleTime(IdleDataManager.GetDataFromFile<IdleTimeVM>(DateTime.UtcNow, IdleDataManager.fileType.data));
                    if (StartTimeResponse < 0) { MessageBox.Show("idle time save error"); }

                    //Idle EndTime entry in Table
                    var endTimeJsonObj = new IdleTimeVM
                    {
                        UserId = IdleDataManager.configData.UserId,
                        EndTime = DateTime.UtcNow,
                        IsDeleted = false,
                        CreatedOn = DateTime.UtcNow,
                        EventType = (int)TaskLogEventType.IdleStop
                    };
                    IdleDataManager.SaveDataToFile(endTimeJsonObj, IdleDataManager.fileType.data);

                    var EndTimeReponse = await IdleDataManager.SaveIdleTime(endTimeJsonObj);
                    if (EndTimeReponse < 0) { MessageBox.Show("idle time save error"); }
                }
                //pause all tasks
                OnSessionUnlock();
                isIdle = false;
                
                
            }
        }

        private void IdleForm_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void IdleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            notifyIcon.Visible = true;
        }


        #region Get Windows Session Events
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_QUERYENDSESSION)
                {
                    OnSessionLock(true);
                    IdleDataManager.ErrorToFile("shutdown/restart", fileType.system_log);

                    systemShutdown = true;
                }
                /*if (m.Msg == SessionChangeMessage)
                {
                    if (m.WParam.ToInt32() == SessionLockParam)
                    {
                        OnSessionLock(true); // save time of windows lock
                    }
                    else if (m.WParam.ToInt32() == SessionUnlockParam)
                    {
                        OnSessionUnlock();
                    }
                    // pause all tasks
                    else
                    {
                        IdleDataManager.ErrorToFile($"any other windows event identified on Form events {m.Msg}", fileType.system_log);
                        OnSessionUnlock();
                    }
                }*/
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }

        }
        protected virtual void OnSessionLock(bool isLogout, bool idleTime = false)
        {
            try
            {
                IdleDataManager.ErrorToFile($"inside on session lock function", fileType.system_log);

                var logoutObj = new LogoutUserVM
                {
                    UserId = IdleDataManager.configData.UserId,
                    LogoutTime = idleTime == true ? DateTime.UtcNow.AddMinutes(-(CommonExtension.IdleTime)) : DateTime.UtcNow,
                    isLogout = isLogout
                };
                IdleDataManager.SaveDataToFile(logoutObj, IdleDataManager.fileType.shutdown_log);
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }
        }

        protected virtual void OnSessionUnlock()
        {
            try
            {
                IdleDataManager.ErrorToFile($"inside on session unlock function", fileType.system_log);
                var logoutDetails = IdleDataManager.GetDataFromFile<LogoutUserVM>(DateTime.UtcNow.Date, IdleDataManager.fileType.shutdown_log);
                if (logoutDetails.UserId != 0)
                    _ = IdleDataManager.SaveLogoutTime(logoutDetails);

            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }

        }
        #endregion


    }
}