using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TMS.Entity;

namespace TimeTrackerExe
{
    public static class LogFileManager
    {

        public static TimeTrackerConfig configData = new();
        private static string DATA_FILE_NAME { get { return $"TMS_Data_{configData.UserId}"; } }
        private static string SYNC_FILE_NAME { get { return $"TMS_Sync_{configData.UserId}"; } }
        static LogFileManager()
        {
            
        }

        static public bool LogTaskEvent(TimeLogReqVM task)
        {
            var data = GetDataFromFile(DateTime.UtcNow) ?? new();
            data.Add(task);
            return SaveDataToFile(data);
        }

        static public List<TimeLogReqVM> GetDataFromFile(DateTime dayToLoad)
        {
            var workPlanList = new List<TimeLogReqVM>();
            var dataFileName = GetDataFileName(dayToLoad);
            if (File.Exists(dataFileName))
            {
                try
                {
                    using StreamReader r = new(dataFileName);
                    string dayToLoadJSON = r.ReadToEnd();
                    workPlanList = !string.IsNullOrEmpty(dayToLoadJSON) ? JsonSerializer.Deserialize<List<TimeLogReqVM>>(dayToLoadJSON) ?? new() : new();
                }
                catch (IOException ex)
                {
                    string msg = string.Format("Error Loading Data File.  No data will be saved until the error is corrected and Application is restarted.  Error: {0}", ex.Message);
                    MessageBox.Show("Error Loading File", msg, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return workPlanList;
        }

        static public bool SaveDataToFile(List<TimeLogReqVM> data)
        {
            var retVal = false;
            var dataFileName = GetDataFileName(DateTime.UtcNow);
            try
            {
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
            catch (IOException ex)
            {
                MessageBox.Show("Error Saving Data File", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return retVal;
        }

        static public void DeleteDataFile(DateTime dayToLoad)
        {
            var dataFileName = GetDataFileName(dayToLoad);
            if (File.Exists(dataFileName))
            {
                File.Delete(dataFileName);
            }
        }

        static private string GetDataFileName(DateTime dayToLoad)
        {
            string dirName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) ?? string.Empty;

            string subDirectory = Path.Combine(dirName, "TMS");

            if (!Directory.Exists(subDirectory))
            {
                Directory.CreateDirectory(subDirectory);
            }

            string baseName = string.Format("{0}_{1}.json", DATA_FILE_NAME, dayToLoad.ToString("yyyyMMdd"));
            return Path.Combine(subDirectory, baseName);
        }

        static private string GetSyncFileName(DateTime dayToLoad)
        {
            string dirName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) ?? string.Empty;

            string subDirectory = Path.Combine(dirName, "TMS");

            if (!Directory.Exists(subDirectory))
            {
                Directory.CreateDirectory(subDirectory);
            }

            string baseName = string.Format("{0}_{1}.json", SYNC_FILE_NAME, dayToLoad.ToString("yyyyMMdd"));
            return Path.Combine(subDirectory, baseName);
        }


        static public bool SaveSyncDataToFile(List<TimeLogReqVM> data)
        {
            var retVal = false;
            var dataFileName = GetSyncFileName(DateTime.UtcNow);
            try
            {
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
                return retVal = true;
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error Saving Data File", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return retVal;
        }

        static public List<TimeLogReqVM> GetDataFromSyncFile(DateTime dayToLoad)
        {
            var workPlanList = new List<TimeLogReqVM>();
            var dataFileName = GetSyncFileName(dayToLoad);
            if (File.Exists(dataFileName))
            {
                try
                {
                    using StreamReader r = new(dataFileName);
                    string dayToLoadJSON = r.ReadToEnd();
                    workPlanList = !string.IsNullOrEmpty(dayToLoadJSON) ? JsonSerializer.Deserialize<List<TimeLogReqVM>>(dayToLoadJSON) ?? new() : new();
                }
                catch (IOException ex)
                {
                    string msg = string.Format("Error Loading Data File.  No data will be saved until the error is corrected and Application is restarted.  Error: {0}", ex.Message);
                    MessageBox.Show("Error Loading File", msg, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return workPlanList;
        }

        static public void DeleteSyncDataFile(DateTime dayToLoad)
        {
            var dataFileName = GetSyncFileName(dayToLoad);
            if (File.Exists(dataFileName))
            {
                File.Delete(dataFileName);
            }
        }
    }
}
