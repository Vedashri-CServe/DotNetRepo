using TMS.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace TimeTrackerExe
{
    public partial class TaskList : Form
    {
        private int? activeTaskIndex = null;

        private bool isIdle = false;
        private TimeSpan idleTimeSpan = new TimeSpan();
        private DataGridViewTextBoxColumn spentTimeCol => (DataGridViewTextBoxColumn)workPlanGridView.Columns["SpentTime"];
        private DataGridViewTextBoxColumn breakTimeCol => (DataGridViewTextBoxColumn)workPlanGridView.Columns["BreakTime"];

        private DataGridViewImageColumn actionBtnCol => (DataGridViewImageColumn)workPlanGridView.Columns["Action"];
        private DataGridViewImageColumn breakBtnCol => (DataGridViewImageColumn)workPlanGridView.Columns["Break"];
        private List<WorkPlanDataItemVM> dataSourceList => (List<WorkPlanDataItemVM>)workPlanGridView.DataSource;

        private DataGridViewImageColumn commentsBtnCol => (DataGridViewImageColumn)workPlanGridView.Columns["Comments"];

        private DataGridViewImageColumn checklistBtnCol => (DataGridViewImageColumn)workPlanGridView.Columns["Checklist"];

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(out LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        public TaskList()
        {
            InitializeComponent();
            workPlanGridView.AutoGenerateColumns = false;
        }

        private async Task SetWorkPlanList()
        {
            Cursor = Cursors.WaitCursor;
            var workPlanFilter = new WorkPlanFilterVM
            {
                GlobalSearch = string.Empty,
                Index = 1,
                PageSize = 1000,
                UserId = LogFileManager.configData.UserId,
                TimelineDate = DateTime.UtcNow
            };
            var dataFileItems = LogFileManager.GetDataFromFile(DateTime.UtcNow);

            var response = await CommonExtension.ExcuteAsync<WorkPlanFilterVM, WorkPlanListWithCountVM>(workPlanFilter, UrlConstants.GetWorkPlanList, RequestType.POST, CommonExtension.GetUserToken());

            if (response?.ResponseStatus == ResponseStatuses.Success)
            {
                var workPlanData = new List<WorkPlanDataItemVM>();
                var respData = response?.ResponseData?.WorkPlanList ?? new();
                respData.ForEach(item =>
                {
                    var totalTime = item.Task?.Label != null ? TimeSpan.FromMinutes((Convert.ToDouble(item.Task.EstimatedDuration)) * item.Quantity) :
                                                               TimeSpan.FromMinutes((Convert.ToDouble(item.Process.EstimatedDuration)) * item.Quantity);  
                    var totalTimeHour = Math.Floor(totalTime.TotalHours);
                    var totalTimeMin = Convert.ToInt32(totalTime.Minutes);

                    var workPlan = new WorkPlanDataItemVM
                    {
                        WorkPlanId = item.WorkPlanId,
                        ClientName = item.Client.Label,
                        ClientId = item.Client.Value,
                        CpaName = item.CPA.Label,
                        CpaId = item.CPA.Value,
                        Quantity = item.Quantity,
                        TaskName = item.Task.Label ?? item.Process.Label,
                        TaskId = item.Task?.Value ?? item.Process.Value,
                        EstimatedTime = item.Task?.Label != null ? string.Format("{0}:{1}", Math.Floor(TimeSpan.FromMinutes(Convert.ToDouble(item.Task.EstimatedDuration)).TotalHours), Convert.ToInt32(TimeSpan.FromMinutes(Convert.ToDouble(item.Task.EstimatedDuration)).Minutes)) :
                                                                   string.Format("{0}:{1}", Math.Floor(TimeSpan.FromMinutes(Convert.ToDouble(item.Process.EstimatedDuration)).TotalHours), Convert.ToInt32(TimeSpan.FromMinutes(Convert.ToDouble(item.Process.EstimatedDuration)).Minutes)),
                        TotalTime =  string.Format("{0}:{1}", totalTimeHour, totalTimeMin),
                        StatusId = item.Status.Value
                    };
                    workPlanData.Add(workPlan);
                });
                workPlanData = LoadDataFromFile(workPlanData);
                workPlanGridView.DataSource = workPlanData;
                FormatCellsAtLoad();
            }
            else
            {
                MessageBox.Show(response?.Message ?? response?.ErrorData?.Error);
            }
            Cursor = Cursors.Default;
        }

        private List<WorkPlanDataItemVM> LoadDataFromFile(List<WorkPlanDataItemVM> data)
        {
            var savedData = LogFileManager.GetDataFromFile(DateTime.UtcNow);
            if (savedData != null && savedData.Count > 0)
            {
                var logEndEvents = new[] { (int)TaskLogEventType.Pause, (int)TaskLogEventType.Stop, (int)TaskLogEventType.BreakEnd };
                var workPlanIds = data.Select(x => x.WorkPlanId);
                var grouped = savedData.Where(x => logEndEvents.Contains(x.EventType) && workPlanIds.Contains(x.WorkPlanId))
                    .GroupBy(x => new { x.WorkPlanId, x.EventType }).SelectMany(x => x.OrderByDescending(x => x.EventTime));
                foreach (var item in data)
                {
                    var saved = grouped.Where(x => x.WorkPlanId == item.WorkPlanId);
                    if (saved == null)
                        continue;
                    if (saved != null && saved.Any())
                    {
                        var lastPause = saved.FirstOrDefault(x => x.EventType == (int)TaskLogEventType.Pause);
                        if (lastPause != null)
                            item.SpentTime = new OffsetStopwatch(TimeSpan.Parse(lastPause.EventDuration));
                        var lastBreak = saved.FirstOrDefault(x => x.EventType == (int)TaskLogEventType.BreakEnd);
                        if (lastBreak != null)
                            item.BreakTime = new OffsetStopwatch(TimeSpan.Parse(lastBreak.EventDuration));
                    }
                }
            }
            return data;
        }

        private void RefreshTimerCells()
        {
            if (workPlanGridView.InvokeRequired)
            {
                workPlanGridView.Invoke((MethodInvoker)delegate ()
                {
                    RefreshTimerCells();
                });
            }
            else if (activeTaskIndex.HasValue)
            {
                var Task = dataSourceList.ElementAt(activeTaskIndex.Value);
                var taskTotalMin = new TimeSpan(0, Convert.ToInt32(Task.TotalTime.Split(":").First()), Convert.ToInt32(Task.TotalTime.Split(":").Last()), 0);
                if (Task.SpentTimeSpan.TotalMinutes > taskTotalMin.TotalMinutes)
                {
                    workPlanGridView.Rows[activeTaskIndex.Value].Cells[spentTimeCol.Index].Style.ForeColor = Color.Red;
                };
                workPlanGridView.UpdateCellValue(spentTimeCol.Index, activeTaskIndex.Value);
                workPlanGridView.UpdateCellValue(breakTimeCol.Index, activeTaskIndex.Value);
           }          
        }

        private void SetTaskRunning(WorkPlanDataItemVM nextTask, int rowIndex)
        {
            if (activeTaskIndex != null && activeTaskIndex.Value != rowIndex)
            {
                workPlanGridView.Rows[activeTaskIndex.Value].DefaultCellStyle.BackColor = Color.White;

                var previousTask = dataSourceList.ElementAt(activeTaskIndex.Value);          
                if (previousTask.SpentTime.IsRunning)
                    SetTaskPause(previousTask);          
                else if (previousTask.BreakTime.IsRunning)
                    SetTaskBreakEnd(previousTask);               
                RefreshTimerCells();
            }
            systemOnBreakLbl.Visible = false;
            activeTaskIndex = rowIndex;
            var hasPreviousRun = nextTask.SpentTime.ElapsedTicks > 0;
            if (nextTask.BreakTime.IsRunning)
                SetTaskBreakEnd(nextTask);
            nextTask.SpentTime.Start();
            LogTaskAction(nextTask, hasPreviousRun ? TaskLogEventType.Continue : TaskLogEventType.Start);
            RefreshTimerCells();

            if(isIdle)
            {
                LogFileManager.LogTaskEvent(new TimeLogReqVM
                {
                    WorkPlanId = nextTask.WorkPlanId,
                    TaskLogId = default,
                    EventType = (int)TaskLogEventType.IdleStart,
                    EventTime = DateTime.UtcNow,
                    EventDuration = idleTimeSpan.ToString()
                });
                isIdle = false;
            }
        }

        private void SetTaskPause(WorkPlanDataItemVM runningTask)
        {
            runningTask.SpentTime.Stop();
            LogTaskAction(runningTask, TaskLogEventType.Pause);
            RefreshTimerCells();
        }

        private void SetTaskOnBreak(WorkPlanDataItemVM workPlan)
        {
            if (workPlan.SpentTime.IsRunning)
                SetTaskPause(workPlan);
            workPlan.BreakTime.Start();
            LogTaskAction(workPlan, TaskLogEventType.BreakStart);
        }

        private void SetTaskBreakEnd(WorkPlanDataItemVM workPlan, int rowIndex = -1)
        {
            workPlan.BreakTime.Stop();
            LogTaskAction(workPlan, TaskLogEventType.BreakEnd);
            if (rowIndex > 0)
                SetTaskRunning(workPlan, rowIndex);
        }

        private void LogTaskAction(WorkPlanDataItemVM task, TaskLogEventType eventType)
        {
            var skipStartLog = new[] { TaskLogEventType.Start, TaskLogEventType.Continue, TaskLogEventType.BreakStart };
            var breakEvent = new[] { TaskLogEventType.BreakStart, TaskLogEventType.BreakEnd };
            LogFileManager.LogTaskEvent(new TimeLogReqVM
            {
                WorkPlanId = task.WorkPlanId,
                TaskLogId = default,
                EventType = (int)eventType,
                EventTime = DateTime.UtcNow,
                EventDuration = skipStartLog.Contains(eventType) ? default : (breakEvent.Contains(eventType) ? task.BreakTime.Elapsed.ToString() : task.SpentTime.Elapsed.ToString())
            });
            RefreshBtnCells(task);
        }

        private void RefreshBtnCells(WorkPlanDataItemVM task)
        {
            var rowIndex = dataSourceList.FindIndex(x => x.WorkPlanId == task.WorkPlanId);
            workPlanGridView.UpdateCellValue(actionBtnCol.Index, rowIndex);
            workPlanGridView.UpdateCellValue(breakBtnCol.Index, rowIndex);
        }

        private void ActionCellClick(int rowIndex)
        {        
            workPlanGridView.Rows[rowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#d8edec");

            var workPlanItem = dataSourceList.ElementAt(rowIndex);
            if (workPlanItem != null)
            {
                if (workPlanItem.SpentTime.IsRunning)
                    SetTaskPause(workPlanItem);
                else
                    SetTaskRunning(workPlanItem, rowIndex);
            }
            GetTotalBreakAndWorkTime();
        }

        private void BreakCellClick(int rowIndex)
        {
            if (activeTaskIndex != null && activeTaskIndex == rowIndex)
            {
                var runningTask = dataSourceList.ElementAt(rowIndex);
                if (runningTask.BreakTime.IsRunning)
                    SetTaskBreakEnd(runningTask, rowIndex);
                else
                    SetTaskOnBreak(runningTask);
            }
            else
                MessageBox.Show("Timer is not started on the task!");
        }

        private void CommentsCellClick(int rowIndex)
        {
            var workPlanItem = dataSourceList.ElementAt(rowIndex);

            CommentList frm = new CommentList(workPlanItem.WorkPlanId);

            frm.ShowDialog();

        }

        private void ChecklistCellClick(int rowIndex)
        {
            var workPlanItem = dataSourceList.ElementAt(rowIndex);

            ChecklistForm frm = new ChecklistForm(workPlanItem.WorkPlanId);

            frm.ShowDialog();

        }

        private async Task SaveTimeLog()
        {
            Cursor = Cursors.WaitCursor;
            var timeLogs = LogFileManager.GetDataFromSyncFile(DateTime.UtcNow).Where(x => x.IsSync == false).ToList();
            if(timeLogs.Count > 0)
            {
                var response = await CommonExtension.ExcuteAsync<List<TimeLogReqVM>, bool>(timeLogs, UrlConstants.SaveTimeLog, RequestType.POST, CommonExtension.GetUserToken());

                if (response?.ResponseStatus == ResponseStatuses.Success)
                {   
                    LogFileManager.DeleteSyncDataFile(DateTime.UtcNow);
                }
                else
                {
                    MessageBox.Show(response?.Message ?? response?.ErrorData?.Error);
                }
            }  
            Cursor = Cursors.Default;
        }

        private async void TaskList_Load(object sender, EventArgs e)
        {
            await SetWorkPlanList();
        }

        private void workPlanGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case var _ when e.ColumnIndex == actionBtnCol.Index:
                    ActionCellClick(e.RowIndex);
                    break;
                case var _ when e.ColumnIndex == commentsBtnCol.Index:
                    CommentsCellClick(e.RowIndex);
                    break;
                case var _ when e.ColumnIndex == checklistBtnCol.Index:
                    ChecklistCellClick(e.RowIndex);
                    break;
                default:
                    break;
            }
        }

        private void spentTimer_Tick(object sender, EventArgs e)
        {
            RefreshTimerCells();
        }

        private async void syncBtn_Click(object sender, EventArgs e)
        {
            var isAnyTaskRunning = dataSourceList.Any(x => x.SpentTime.IsRunning);
            if (!isAnyTaskRunning)
            {
                await SaveTimeLog();
            }
            else
            {
                MessageBox.Show(MessageConstants.TaskRunning);
            }
        }

        private void FormatCellsAtLoad()
        {
            for(int i = 0; i< dataSourceList.Count; i++)
            {
                var Task = dataSourceList[i];
                var taskTotalMin = new TimeSpan(0, Convert.ToInt32(Task.TotalTime.Split(":").First()), Convert.ToInt32(Task.TotalTime.Split(":").Last()), 0);
                if (Task.SpentTimeSpan.TotalMinutes > taskTotalMin.TotalMinutes)
                {
                    workPlanGridView.Rows[i].Cells[spentTimeCol.Index].Style.ForeColor = Color.Red;
                };

            }
            GetTotalBreakAndWorkTime();
        }

        private void breakBtn_Click(object sender, EventArgs e)
        {           
            if (activeTaskIndex.HasValue)
            {
                systemOnBreakLbl.Visible = true;
                var Task = dataSourceList.ElementAt(activeTaskIndex.Value);
                BreakCellClick(activeTaskIndex.Value);
                GetTotalBreakAndWorkTime();
            }
            else
                MessageBox.Show("Timer is not started on any task!");
        }

        private void GetTotalBreakAndWorkTime()
        {
            TimeSpan TotalBreakTime = new TimeSpan();
            TimeSpan TotalWorkTime = new TimeSpan();
            var savedData = LogFileManager.GetDataFromFile(DateTime.UtcNow);
            if (savedData != null && savedData.Count > 0)
            {
               
                foreach (var item in dataSourceList)
                {
                    var breakTime = item.BreakTimeSpan;
                    TotalBreakTime = TotalBreakTime.Add(breakTime);

                    var workTime = item.SpentTimeSpan;
                    TotalWorkTime = TotalWorkTime.Add(workTime);
                   
                }
            }

            totalBreakLbl.Text = TotalBreakTime.ToString(@"hh\:mm\:ss");
            totalWorkLbl.Text = TotalWorkTime.ToString(@"hh\:mm\:ss");
        }

        private async void syncTimer_Tick(object sender, EventArgs e)
        {
            var savedData = LogFileManager.GetDataFromFile(DateTime.UtcNow);
            if (savedData != null && savedData.Count > 0)
            {

                var check = LogFileManager.SaveSyncDataToFile(savedData);
                var syncedData = LogFileManager.GetDataFromSyncFile(DateTime.UtcNow);

                if (check && savedData.Where(x => x.IsSync == false).Any())
                {
                    await SaveTimeLog();
                    var newsavedData = LogFileManager.GetDataFromFile(DateTime.UtcNow);

                    var commonItems = newsavedData.Where(p => savedData.Any(l => p.TaskLogId == l.TaskLogId)).ToList();
                    foreach (var item in commonItems) { item.IsSync = true; }

                    var tempResult = savedData.Where(p => newsavedData.Any(l => p.TaskLogId != l.TaskLogId)).ToList();
                    var finalResult = tempResult.Union(commonItems).ToList();

                    LogFileManager.SaveDataToFile(finalResult);

                }
            }

        }

        private void idleTimer_Tick(object sender, EventArgs e)
        {

            if(systemOnBreakLbl.Visible == false) 
            {
                LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
                lastInputInfo.cbSize = (uint)LASTINPUTINFO.SizeOf;
                GetLastInputInfo(out lastInputInfo);

                int elapsedTicks = Environment.TickCount - (int)lastInputInfo.dwTime;
                var res = new TimeSpan(0, 0, 0, 0, elapsedTicks);
                idleTimeSpan = idleTimeSpan.Add(res);
                var idleTime = elapsedTicks / 60000;
                if (idleTime >= 2 && activeTaskIndex.HasValue && isIdle == false)
                {
                     isIdle = true;
                    
                    var workPlanItem = dataSourceList.ElementAt(activeTaskIndex.Value);
                    if (workPlanItem != null)
                    {
                        if (workPlanItem.SpentTime.IsRunning)
                            SetTaskPause(workPlanItem);
                    }
                    GetTotalBreakAndWorkTime();

                    LogFileManager.LogTaskEvent(new TimeLogReqVM
                    {
                        WorkPlanId = workPlanItem.WorkPlanId,
                        TaskLogId = default,
                        EventType = (int)TaskLogEventType.IdleStart,
                        EventTime = DateTime.UtcNow,
                        EventDuration = idleTimeSpan.ToString()
                    });
                }
            }
            
        }
    }
}
