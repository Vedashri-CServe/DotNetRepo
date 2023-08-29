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
    public partial class ChecklistForm : Form
    {
        private long checklistWorkplanId { get; set; }
        public ChecklistForm(long checklistWorkplanId)
        {
            InitializeComponent();
            this.checklistWorkplanId = checklistWorkplanId;
        }

        private async void ChecklistForm_Load(object sender, EventArgs e)
        {
            await SetWorkplanCheckList();
        }

        public async Task SetWorkplanCheckList()
        {
            Cursor = Cursors.WaitCursor;
            var filter = new checkListReqId
            {
                WorkPlanId = checklistWorkplanId
            };

            var response = await CommonExtension.ExcuteAsync<checkListReqId, List<CheckListVM>>(filter, UrlConstants.GetCheckList, RequestType.POST, CommonExtension.GetUserToken());

            if (response?.ResponseStatus == ResponseStatuses.Success)
            {
                var checklistData = new List<WorkplanChecklistResponseVM>();
                var respData = response?.ResponseData ?? new();

                long Id = 0;

                if (respData.Count > 0)
                {
                    respData.ForEach(item =>
                    {
                        var checklist = new WorkplanChecklistResponseVM
                        {
                            Id = ++Id,
                            Description = item.Description.Trim(),
                            Checked = item.IsChecked
                        };
                        checklistData.Add(checklist);
                    });
                }
                checklistGridView.DataSource = checklistData;

            }
            else
            {
                MessageBox.Show(response?.Message ?? response?.ErrorData?.Error);
            }
            Cursor = Cursors.Default;
        }

        private void checklistGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
