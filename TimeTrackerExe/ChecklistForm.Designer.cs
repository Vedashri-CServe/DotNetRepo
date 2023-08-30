namespace TimeTrackerExe
{
    partial class ChecklistForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChecklistForm));
            this.checklistGridView = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.checklistGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // checklistGridView
            // 
            this.checklistGridView.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            this.checklistGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ScrollBar;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.checklistGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.checklistGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.checklistGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Description,
            this.Checked});
            this.checklistGridView.EnableHeadersVisualStyles = false;
            this.checklistGridView.GridColor = System.Drawing.SystemColors.HighlightText;
            this.checklistGridView.Location = new System.Drawing.Point(1, 4);
            this.checklistGridView.Name = "checklistGridView";
            this.checklistGridView.RowHeadersVisible = false;
            this.checklistGridView.RowTemplate.Height = 25;
            this.checklistGridView.Size = new System.Drawing.Size(430, 150);
            this.checklistGridView.TabIndex = 0;
            this.checklistGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.checklistGridView_CellContentClick);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Width = 50;
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Description";
            this.Description.MinimumWidth = 50;
            this.Description.Name = "Description";
            this.Description.Width = 300;
            // 
            // Checked
            // 
            this.Checked.DataPropertyName = "Checked";
            this.Checked.HeaderText = "Checked";
            this.Checked.Name = "Checked";
            this.Checked.ReadOnly = true;
            this.Checked.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Checked.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Checked.Width = 75;
            // 
            // ChecklistForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(433, 166);
            this.Controls.Add(this.checklistGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ChecklistForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ChecklistForm";
            this.Load += new System.EventHandler(this.ChecklistForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.checklistGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView checklistGridView;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewCheckBoxColumn Checked;
    }
}