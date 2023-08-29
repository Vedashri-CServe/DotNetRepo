namespace TimeTrackerExe
{
    partial class CommentList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommentList));
            this.commentGridView = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommentBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.commentGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // commentGridView
            // 
            this.commentGridView.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            this.commentGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ScrollBar;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.commentGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.commentGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.commentGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.CommentBy,
            this.Comment,
            this.CreatedOn});
            this.commentGridView.EnableHeadersVisualStyles = false;
            this.commentGridView.GridColor = System.Drawing.SystemColors.HighlightText;
            this.commentGridView.Location = new System.Drawing.Point(0, 1);
            this.commentGridView.Name = "commentGridView";
            this.commentGridView.RowHeadersVisible = false;
            this.commentGridView.RowTemplate.Height = 25;
            this.commentGridView.Size = new System.Drawing.Size(591, 150);
            this.commentGridView.TabIndex = 0;
            this.commentGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.commentGridView_CellContentClick);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "id";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.Width = 50;
            // 
            // CommentBy
            // 
            this.CommentBy.DataPropertyName = "commentBy";
            this.CommentBy.HeaderText = "Comment By";
            this.CommentBy.MinimumWidth = 20;
            this.CommentBy.Name = "CommentBy";
            this.CommentBy.Width = 135;
            // 
            // Comment
            // 
            this.Comment.DataPropertyName = "comment";
            this.Comment.HeaderText = "Comment";
            this.Comment.MinimumWidth = 30;
            this.Comment.Name = "Comment";
            this.Comment.Width = 300;
            // 
            // CreatedOn
            // 
            this.CreatedOn.DataPropertyName = "CreatedOn";
            this.CreatedOn.HeaderText = "Created On";
            this.CreatedOn.Name = "CreatedOn";
            // 
            // CommentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(587, 177);
            this.Controls.Add(this.commentGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CommentList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CommentList";
            this.Load += new System.EventHandler(this.CommentList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.commentGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView commentGridView;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn CommentBy;
        private DataGridViewTextBoxColumn Comment;
        private DataGridViewTextBoxColumn CreatedOn;
    }
}