using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using TMS.Auth;
using TMS.Entity;

namespace TimeTrackerExe
{
    public class IdleDataManager
    {
        public static TimeTrackerConfig configData = new();
        private static string DATA_FILE_NAME { get { return $"TMS_Data_{configData.UserId}"; } }
        private static string ERROR_FILE_NAME { get { return $"TMS_ErrorLog_{configData.UserId}"; } }
        private static string CRED_FILE_NAME { get { return $"TMS_CRED"; } }
        private static string SHUTDOWN_LOG_FILE_NAME { get { return $"TMS_ShutDownLog_{configData.UserId}"; } }
        private static string SETUP_LOCATION { get { return $"TMS_SETUP_{configData.UserId}"; } }
        private static string SYSTEM_LOG { get { return $"TMS_LOG_{configData.UserId}"; } }
        public enum fileType
        {
            data,
            error,
            credentials,
            shutdown_log,
            setup,
            system_log
        }

        #region API calls
        public static async Task<int> SaveIdleTime(IdleTimeVM idleTime)
        {
            try
            {
                var response = await CommonExtension.ExcuteAsync<IdleTimeVM, int>(idleTime, UrlConstants.SaveIdleTime, RequestType.POST, CommonExtension.GetUserToken());
                return response.ResponseData;
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }
            return -1;
        }

        public static async Task<int> GetLastEventTypeByUserId(long UserId)
        {
            try
            {
                var response = await CommonExtension.ExcuteAsync<long, int>(UserId, UrlConstants.GetLastEventTypeByUserId, RequestType.POST, CommonExtension.GetUserToken());
                return response.ResponseData;

            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }
            return -1;
        }

        public static async Task<long> SaveLogoutTime(LogoutUserVM logoutUser)
        {
            try
            {
                var response = await CommonExtension.ExcuteAsync<LogoutUserVM, long>(logoutUser, UrlConstants.SaveLogoutUser, RequestType.POST, CommonExtension.GetUserToken());
                return response.ResponseData;
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
                return -1;
            }
        }
        #endregion

        static public bool ErrorToFile(dynamic data, fileType type)
        {
            var retVal = false;
            var dataFileName = GetDataFileName(DateTime.UtcNow, type);
            try
            {
                using StreamWriter dataFile = new(dataFileName, Encoding.UTF8, new FileStreamOptions
                {
                    Mode = FileMode.Append,
                    Access = FileAccess.Write,
                    Options = FileOptions.SequentialScan,
                });
                dataFile.WriteLine(data + " " + DateTime.UtcNow.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
                MessageBox.Show("Error Saving Data File", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return retVal;
        }
        static public string GetDataFileName(DateTime dayToLoad, fileType type)
        {
            string dirName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) ?? string.Empty;

            string subDirectory = Path.Combine(dirName, "TMS");
            string baseName = string.Empty;
            if (!Directory.Exists(subDirectory))
            {
                Directory.CreateDirectory(subDirectory);
            }

            if (type.Equals(fileType.data))
            {
                baseName = string.Format("{0}.json", DATA_FILE_NAME);
            }
            else if (type.Equals(fileType.error))
            {
                baseName = string.Format("{0}_{1}.json", ERROR_FILE_NAME, dayToLoad.ToString("yyyyMMdd"));
            }
            else if (type.Equals(fileType.credentials))
            {
                baseName = string.Format("{0}.json", CRED_FILE_NAME);
            }
            else if (type.Equals(fileType.shutdown_log))
            {
                baseName = string.Format("{0}.json", SHUTDOWN_LOG_FILE_NAME);
            }
            else if (type.Equals(fileType.system_log))
                baseName = string.Format("{0}_{1}.json", SYSTEM_LOG, dayToLoad.ToString("yyyyMMdd"));
            else if (type.Equals(fileType.setup))
                return Path.Combine(subDirectory, baseName);
            return Path.Combine(subDirectory, baseName);
        }

        static public bool SaveDataToFile(dynamic data, fileType type)
        {
            var retVal = false;
            var dataFileName = GetDataFileName(DateTime.UtcNow, type);
            try
            {
                IdleDataManager.ErrorToFile($"inside SaveDataToFile function", fileType.system_log);

                var dataJSONString = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                });
                using StreamWriter dataFile = new(dataFileName, Encoding.UTF8, new FileStreamOptions
                {
                    Mode = FileMode.Create,
                    Access = FileAccess.Write,
                    Options = FileOptions.SequentialScan
                });
                dataFile.WriteLine(dataJSONString);
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
                MessageBox.Show("Error Saving Data File", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return retVal;
        }

        static public T GetDataFromFile<T>(DateTime dayToLoad, fileType type) where T : class, new()
        {
            var detailsObject = Activator.CreateInstance<T>();
            try
            {
                var dataFileName = GetDataFileName(dayToLoad, type);
                if (File.Exists(dataFileName))
                {
                    try
                    {
                        using StreamReader r = new(dataFileName);
                        string dayToLoadJSON = r.ReadToEnd();
                        detailsObject = !string.IsNullOrEmpty(dayToLoadJSON) ? JsonSerializer.Deserialize<T>(dayToLoadJSON) ?? new() : new();
                    }
                    catch (Exception ex)
                    {
                        string msg = string.Format("Error Loading Data File.  No data will be saved until the error is corrected and Application is restarted.  Error: {0}", ex.Message);
                        MessageBox.Show("Error Loading File", msg, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }
            return detailsObject;
        }

        static public UserCredentials GetUserCredentials(fileType type)
        {
            UserCredentials userCredentials = new UserCredentials();
            try
            {
                var dataFileName = GetDataFileName(DateTime.UtcNow, type);

                if (File.Exists(dataFileName))
                {
                    using StreamReader r = new(dataFileName);
                    string userCredJSON = r.ReadToEnd();
                    userCredentials = !string.IsNullOrEmpty(userCredJSON) ? JsonSerializer.Deserialize<UserCredentials>(userCredJSON) ?? new() : null;
                    return userCredentials;

                }
                return null;
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }
            return userCredentials;
        }

        public static async Task<BlobRes> GetSetupFileLocation()
        {
            IdleDataManager.ErrorToFile($"inside get setup file location funct", fileType.system_log);

            var config = new DownloadExeSetup { isOnlyVideos = false };
            try
            {
                var response = await CommonExtension.ExcuteAsync<DownloadExeSetup, IEnumerable<BlobRes>>(config, UrlConstants.GetExeSetupLocation, RequestType.POST, string.Empty);
                var blobStorage = response.ResponseData?.SingleOrDefault();
                return blobStorage;
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
                return null;
            }

        }

        public static string FindFileLoc(string directory, string fileName)
        {
            string foundFileName = string.Empty;
            try
            {
                foundFileName = Directory.GetFiles(directory, fileName).FirstOrDefault();
                if (String.IsNullOrEmpty(foundFileName))
                {
                    foreach (string dir in Directory.GetDirectories(directory))
                    {
                        foundFileName = FindFileLoc(dir, fileName);
                        if (!String.IsNullOrEmpty(foundFileName))
                            break;
                    }
                }
            }
            catch (Exception ex) // The most likely exception is UnauthorizedAccessException
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }
            return foundFileName;
        }

        /*public static bool CheckForUpdate()
        {
            Version NewVersionNumber = new Version();
            Version OldVersionNumber = new Version();

            //get tms setup zip from Azure Blob storage
            var blobConfigData = GetSetupFileLocation();
            if (blobConfigData != null)
            {
                var client = new WebClient();

                var downloadSetupAt = GetDataFileName(DateTime.UtcNow, fileType.setup);
                string zipPath = $"{downloadSetupAt}\\TMSSetup.zip";
                if (File.Exists(zipPath)) { File.Delete(zipPath); }

                client.DownloadFile(blobConfigData.Result.Url!, zipPath);

                //get Version from latest downloaded zip file
                using var zipFile = ZipFile.OpenRead(zipPath);
                foreach (var entry in zipFile.Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Name) && entry.Name.Contains("Version"))
                    {
                        using (var stream = entry.Open())
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            var bytes = memoryStream.ToArray();
                            NewVersionNumber = ParseVersion(Encoding.UTF8.GetString(bytes, 0, bytes.Length).ToString());
                        }
                    }
                }

                // check if version is updated
                var VersionPath = FindFileLoc(downloadSetupAt, "Version.txt");

                if (!string.IsNullOrEmpty(VersionPath))
                {
                    using (StreamReader reader = new StreamReader(VersionPath))
                    { OldVersionNumber = ParseVersion(Convert.ToString(reader.ReadLine())); }
                }
                //if version differs extract latest zip
                if (!string.IsNullOrEmpty(NewVersionNumber.ToString()) && (OldVersionNumber.CompareTo(NewVersionNumber) == -1))
                {
                    if (MessageBox.Show("A new update is available,click on the \"OK\" button.", "TMS Application Update", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) == DialogResult.OK)
                    {
                        string extractPath = downloadSetupAt;
                        ZipFile.ExtractToDirectory(zipPath, extractPath, true);

                        string TMSSetup = FindFileLoc(downloadSetupAt, "setup.exe");

                        try
                        {
                            //start installer
                            Process extract = new Process();
                            extract.StartInfo.UseShellExecute = false;
                            extract.StartInfo.CreateNoWindow = false;
                            extract.StartInfo.RedirectStandardOutput = true;
                            extract.StartInfo.FileName = TMSSetup;
                            extract.Start();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            IdleDataManager.ErrorToFile(ex, fileType.system_log);
                        }
                    }

                }
            }
            return false;


        private static Version ParseVersion(string input)
        {
            IdleDataManager.ErrorToFile($"inside parse version function", fileType.system_log);

            Version ver = null;
            try
            {
                if (Version.TryParse(input, out ver))
                    return ver;
                else
                    return null;
            }
            catch (Exception ex)
            {
                IdleDataManager.ErrorToFile(ex, fileType.system_log);
            }
            return ver;
        }
                }
*/

    }
}