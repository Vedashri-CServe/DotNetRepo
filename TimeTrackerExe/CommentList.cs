using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMS.Entity;

namespace TimeTrackerExe
{
    public partial class CommentList : Form
    {

        private long commentWorkplanId { get; set; }
        public CommentList(long CommentWorkplanId)
        {
            InitializeComponent();
            this.commentWorkplanId = CommentWorkplanId;
        }

        private async void CommentList_Load(object sender, EventArgs e)
        {
            await SetWorkplanCommentList();
        }

        private async Task SetWorkplanCommentList()
        {
            Cursor = Cursors.WaitCursor;
            var filter = new checkListReqId
            {
                WorkPlanId = commentWorkplanId
            };

            var response = await CommonExtension.ExcuteAsync<checkListReqId, WorkPlanCommentListWithCountVM>(filter, UrlConstants.GetWorkPlanComment, RequestType.POST, CommonExtension.GetUserToken());

            if (response?.ResponseStatus == ResponseStatuses.Success)
            {
                var commentData = new List<WorkplanCommentResponseVM>();
                var respData = response?.ResponseData?.CommentList ?? new();

                long Id = 0;

                if(respData.Count > 0){

                    respData.ForEach(item =>
                    {
                        var comments = new WorkplanCommentResponseVM
                        {
                            Id = ++Id,
                            CreatedOn = item.CreatedOn,
                            CommentBy = item.CommentBy.ToString().Trim(),
                            Comment = item.Comment.ToString().Trim()
                        };
                        commentData.Add(comments);
                    });
                }
                

                commentGridView.DataSource = commentData;

            }
            else
            {
                MessageBox.Show(response?.Message ?? response?.ErrorData?.Error);
            }
            Cursor = Cursors.Default;
        }

        private void commentGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
