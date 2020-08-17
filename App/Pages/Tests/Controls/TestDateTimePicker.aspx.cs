using System;
using System.Web.UI;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Components;

namespace App.Tests
{
    /// <summary>
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class TestDateTimePicker : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.dtp.SelectedDate = DateTime.Now;
                this.dtp2.SelectedDate = DateTime.Now.AddDays(30);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            labResult.Text = String.Format("日期1：{0}<br/>日期2：{1}<br/>日期3：{2}",
                dp.Text,
                dtp.Text,
                dtp2.SelectedDate
                );
        }
    }
}
