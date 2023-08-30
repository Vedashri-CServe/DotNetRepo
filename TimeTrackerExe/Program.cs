using System.Diagnostics;
using TMS.Entity;

namespace TimeTrackerExe
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Guid TMSGuid = Guid.NewGuid();
            string TMS = "TMSSetup";
            bool instanceCount = false;
           // bool updateAvailable = false;
            try
            {
                using (Mutex mutex = new(true, TMS, out instanceCount))
                {
                    if (instanceCount)
                    {
                        ApplicationConfiguration.Initialize();
                        Application.Run(new LoginForm());
                       // updateAvailable = IdleDataManager.CheckForUpdate();
                       //  if (!updateAvailable) { Application.Run(new LoginForm()); }
                        mutex.ReleaseMutex();
                    }
                    else
                    {
                        MessageBox.Show("An application instance is already running");
                    }

                }
            }
            catch(Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, IdleDataManager.fileType.system_log);
            }

        }

        
    }
}