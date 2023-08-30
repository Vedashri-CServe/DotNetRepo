namespace TimeTrackerExe
{
    partial class TaskList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskList));
            this.workPlanGridView = new System.Windows.Forms.DataGridView();
            this.WorkPlanId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cpa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Client = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Task = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EstimatedTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SpentTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BreakTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Action = new System.Windows.Forms.DataGridViewImageColumn();
            this.Break = new System.Windows.Forms.DataGridViewImageColumn();
            this.Comments = new System.Windows.Forms.DataGridViewImageColumn();
            this.Checklist = new System.Windows.Forms.DataGridViewImageColumn();
            this.spentTimer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.systemOnBreakLbl = new System.Windows.Forms.Label();
            this.totalBreakLbl = new System.Windows.Forms.Label();
            this.totalWorkLbl = new System.Windows.Forms.Label();
            this.totalWork = new System.Windows.Forms.Label();
            this.totalBreak = new System.Windows.Forms.Label();
            this.breakBtn = new System.Windows.Forms.Button();
            this.syncBtn = new System.Windows.Forms.Button();
            this.syncTimer = new System.Windows.Forms.Timer(this.components);
            this.idleTimer = new System.Windows.Forms.Timer(this.components);
            this.idleTimeData = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.workPlanGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // workPlanGridView
            // 
            this.workPlanGridView.AllowUserToDeleteRows = false;
            this.workPlanGridView.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            this.workPlanGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.workPlanGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ScrollBar;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.MenuText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.workPlanGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.workPlanGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.workPlanGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WorkPlanId,
            this.StatusId,
            this.Cpa,
            this.Client,
            this.Task,
            this.EstimatedTime,
            this.Quantity,
            this.TotalTime,
            this.SpentTime,
            this.BreakTime,
            this.Action,
            this.Break,
            this.Comments,
            this.Checklist});
            this.workPlanGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workPlanGridView.EnableHeadersVisualStyles = false;
            this.workPlanGridView.GridColor = System.Drawing.SystemColors.HighlightText;
            this.workPlanGridView.Location = new System.Drawing.Point(0, 0);
            this.workPlanGridView.Name = "workPlanGridView";
            this.workPlanGridView.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.ScrollBar;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.workPlanGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.workPlanGridView.RowHeadersVisible = false;
            this.workPlanGridView.RowTemplate.Height = 25;
            this.workPlanGridView.Size = new System.Drawing.Size(1001, 388);
            this.workPlanGridView.TabIndex = 0;
            this.workPlanGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.workPlanGridView_CellContentClick);
            // 
            // WorkPlanId
            // 
            this.WorkPlanId.DataPropertyName = "WorkPlanId";
            this.WorkPlanId.HeaderText = "Id";
            this.WorkPlanId.Name = "WorkPlanId";
            this.WorkPlanId.ReadOnly = true;
            this.WorkPlanId.Visible = false;
            // 
            // StatusId
            // 
            this.StatusId.DataPropertyName = "StatusId";
            this.StatusId.HeaderText = "Status";
            this.StatusId.Name = "StatusId";
            this.StatusId.ReadOnly = true;
            this.StatusId.Visible = false;
            // 
            // Cpa
            // 
            this.Cpa.DataPropertyName = "CpaName";
            this.Cpa.HeaderText = "CPA";
            this.Cpa.Name = "Cpa";
            this.Cpa.ReadOnly = true;
            // 
            // Client
            // 
            this.Client.DataPropertyName = "ClientName";
            this.Client.HeaderText = "Clients";
            this.Client.Name = "Client";
            this.Client.ReadOnly = true;
            // 
            // Task
            // 
            this.Task.DataPropertyName = "TaskName";
            this.Task.HeaderText = "Tasks";
            this.Task.Name = "Task";
            this.Task.ReadOnly = true;
            // 
            // EstimatedTime
            // 
            this.EstimatedTime.DataPropertyName = "EstimatedTime";
            dataGridViewCellStyle2.Format = "hh\':\'mm";
            this.EstimatedTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.EstimatedTime.HeaderText = "Estimated Time";
            this.EstimatedTime.Name = "EstimatedTime";
            this.EstimatedTime.ReadOnly = true;
            // 
            // Quantity
            // 
            this.Quantity.DataPropertyName = "Quantity";
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.Name = "Quantity";
            this.Quantity.ReadOnly = true;
            // 
            // TotalTime
            // 
            this.TotalTime.DataPropertyName = "TotalTime";
            dataGridViewCellStyle3.Format = "hh\':\'mm";
            this.TotalTime.DefaultCellStyle = dataGridViewCellStyle3;
            this.TotalTime.HeaderText = "Total Time";
            this.TotalTime.Name = "TotalTime";
            this.TotalTime.ReadOnly = true;
            // 
            // SpentTime
            // 
            this.SpentTime.DataPropertyName = "SpentTimeSpan";
            dataGridViewCellStyle4.Format = "hh\':\'mm\':\'ss";
            this.SpentTime.DefaultCellStyle = dataGridViewCellStyle4;
            this.SpentTime.HeaderText = "Time Spent";
            this.SpentTime.Name = "SpentTime";
            this.SpentTime.ReadOnly = true;
            // 
            // BreakTime
            // 
            this.BreakTime.DataPropertyName = "BreakTimeSpan";
            dataGridViewCellStyle5.Format = "hh\':\'mm\':\'ss";
            this.BreakTime.DefaultCellStyle = dataGridViewCellStyle5;
            this.BreakTime.HeaderText = "Break Time";
            this.BreakTime.Name = "BreakTime";
            this.BreakTime.ReadOnly = true;
            this.BreakTime.Visible = false;
            // 
            // Action
            // 
            this.Action.DataPropertyName = "ActionBtnLbl";
            this.Action.HeaderText = "Actions";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            this.Action.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Break
            // 
            this.Break.DataPropertyName = "BreakBtnLbl";
            this.Break.HeaderText = "Break";
            this.Break.Name = "Break";
            this.Break.ReadOnly = true;
            this.Break.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Break.Visible = false;
            // 
            // Comments
            // 
            this.Comments.DataPropertyName = "CommentsBtnLabel";
            this.Comments.HeaderText = "Comments";
            this.Comments.Image = global::TimeTrackerExe.Properties.Resources.comment1;
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Checklist
            // 
            this.Checklist.DataPropertyName = "ChecklistBtnLbl";
            this.Checklist.HeaderText = "Checklist";
            this.Checklist.Image = global::TimeTrackerExe.Properties.Resources.checklisticon1;
            this.Checklist.Name = "Checklist";
            this.Checklist.ReadOnly = true;
            this.Checklist.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // spentTimer
            // 
            this.spentTimer.Enabled = true;
            this.spentTimer.Interval = 1000;
            this.spentTimer.Tick += new System.EventHandler(this.spentTimer_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.workPlanGridView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.splitContainer1.Panel2.Controls.Add(this.idleTimeData);
            this.splitContainer1.Panel2.Controls.Add(this.systemOnBreakLbl);
            this.splitContainer1.Panel2.Controls.Add(this.totalBreakLbl);
            this.splitContainer1.Panel2.Controls.Add(this.totalWorkLbl);
            this.splitContainer1.Panel2.Controls.Add(this.totalWork);
            this.splitContainer1.Panel2.Controls.Add(this.totalBreak);
            this.splitContainer1.Panel2.Controls.Add(this.breakBtn);
            this.splitContainer1.Panel2.Controls.Add(this.syncBtn);
            this.splitContainer1.Size = new System.Drawing.Size(1001, 440);
            this.splitContainer1.SplitterDistance = 388;
            this.splitContainer1.TabIndex = 1;
            // 
            // systemOnBreakLbl
            // 
            this.systemOnBreakLbl.AutoSize = true;
            this.systemOnBreakLbl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.systemOnBreakLbl.ForeColor = System.Drawing.Color.IndianRed;
            this.systemOnBreakLbl.Location = new System.Drawing.Point(763, 19);
            this.systemOnBreakLbl.Name = "systemOnBreakLbl";
            this.systemOnBreakLbl.Size = new System.Drawing.Size(127, 16);
            this.systemOnBreakLbl.TabIndex = 7;
            this.systemOnBreakLbl.Text = "System is on break";
            this.systemOnBreakLbl.Visible = false;
            // 
            // totalBreakLbl
            // 
            this.totalBreakLbl.AutoSize = true;
            this.totalBreakLbl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.totalBreakLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(45)))), ((int)(((byte)(39)))));
            this.totalBreakLbl.Location = new System.Drawing.Point(700, 19);
            this.totalBreakLbl.Name = "totalBreakLbl";
            this.totalBreakLbl.Size = new System.Drawing.Size(57, 16);
            this.totalBreakLbl.TabIndex = 6;
            this.totalBreakLbl.Text = "00:00:00";
            // 
            // totalWorkLbl
            // 
            this.totalWorkLbl.AutoSize = true;
            this.totalWorkLbl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.totalWorkLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(45)))), ((int)(((byte)(39)))));
            this.totalWorkLbl.Location = new System.Drawing.Point(398, 18);
            this.totalWorkLbl.Name = "totalWorkLbl";
            this.totalWorkLbl.Size = new System.Drawing.Size(57, 16);
            this.totalWorkLbl.TabIndex = 5;
            this.totalWorkLbl.Text = "00:00:00";
            // 
            // totalWork
            // 
            this.totalWork.AutoSize = true;
            this.totalWork.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.totalWork.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(45)))), ((int)(((byte)(39)))));
            this.totalWork.Location = new System.Drawing.Point(255, 17);
            this.totalWork.Name = "totalWork";
            this.totalWork.Size = new System.Drawing.Size(119, 19);
            this.totalWork.TabIndex = 4;
            this.totalWork.Text = "Worked Today";
            // 
            // totalBreak
            // 
            this.totalBreak.AutoSize = true;
            this.totalBreak.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.totalBreak.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(45)))), ((int)(((byte)(39)))));
            this.totalBreak.Location = new System.Drawing.Point(543, 17);
            this.totalBreak.Name = "totalBreak";
            this.totalBreak.Size = new System.Drawing.Size(136, 19);
            this.totalBreak.TabIndex = 3;
            this.totalBreak.Text = "Total Break Time";
            // 
            // breakBtn
            // 
            this.breakBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(196)))), ((int)(((byte)(172)))));
            this.breakBtn.FlatAppearance.BorderSize = 0;
            this.breakBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.breakBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.breakBtn.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.breakBtn.Location = new System.Drawing.Point(893, 13);
            this.breakBtn.Margin = new System.Windows.Forms.Padding(0);
            this.breakBtn.Name = "breakBtn";
            this.breakBtn.Size = new System.Drawing.Size(75, 26);
            this.breakBtn.TabIndex = 2;
            this.breakBtn.Text = "Break";
            this.breakBtn.UseVisualStyleBackColor = false;
            this.breakBtn.Click += new System.EventHandler(this.breakBtn_Click);
            // 
            // syncBtn
            // 
            this.syncBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(196)))), ((int)(((byte)(172)))));
            this.syncBtn.FlatAppearance.BorderSize = 0;
            this.syncBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.syncBtn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.syncBtn.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.syncBtn.Location = new System.Drawing.Point(45, 13);
            this.syncBtn.Margin = new System.Windows.Forms.Padding(0);
            this.syncBtn.Name = "syncBtn";
            this.syncBtn.Size = new System.Drawing.Size(75, 26);
            this.syncBtn.TabIndex = 0;
            this.syncBtn.Text = "Sync";
            this.syncBtn.UseVisualStyleBackColor = false;
            this.syncBtn.Visible = false;
            this.syncBtn.Click += new System.EventHandler(this.syncBtn_Click);
            // 
            // syncTimer
            // 
            this.syncTimer.Enabled = true;
            this.syncTimer.Interval = 120000;
            this.syncTimer.Tick += new System.EventHandler(this.syncTimer_Tick);
            // 
            // idleTimer
            // 
            this.idleTimer.Enabled = true;
            this.idleTimer.Interval = 60000;
            this.idleTimer.Tick += new System.EventHandler(this.idleTimer_Tick);
            // 
            // idleTimeData
            // 
            this.idleTimeData.AutoSize = true;
            this.idleTimeData.Location = new System.Drawing.Point(146, 17);
            this.idleTimeData.Name = "idleTimeData";
            this.idleTimeData.Size = new System.Drawing.Size(0, 15);
            this.idleTimeData.TabIndex = 8;
            // 
            // TaskList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(1001, 440);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TaskList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TaskList";
            this.Load += new System.EventHandler(this.TaskList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.workPlanGridView)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView workPlanGridView;
        private System.Windows.Forms.Timer spentTimer;
        private SplitContainer splitContainer1;
        private Button syncBtn;
        private Button breakBtn;
        private Label totalBreakLbl;
        private Label totalWorkLbl;
        private Label totalWork;
        private Label totalBreak;
        private DataGridViewTextBoxColumn WorkPlanId;
        private DataGridViewTextBoxColumn StatusId;
        private DataGridViewTextBoxColumn Cpa;
        private DataGridViewTextBoxColumn Client;
        private DataGridViewTextBoxColumn Task;
        private DataGridViewTextBoxColumn EstimatedTime;
        private DataGridViewTextBoxColumn Quantity;
        private DataGridViewTextBoxColumn TotalTime;
        private DataGridViewTextBoxColumn SpentTime;
        private DataGridViewTextBoxColumn BreakTime;
        private DataGridViewImageColumn Action;
        private DataGridViewImageColumn Break;
        private DataGridViewImageColumn Comments;
        private DataGridViewImageColumn Checklist;
        private System.Windows.Forms.Timer syncTimer;
        private Label systemOnBreakLbl;
        private System.Windows.Forms.Timer idleTimer;
        private Label idleTimeData;
    }
}