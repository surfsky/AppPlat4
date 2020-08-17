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
    public partial class TestDateTime : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.tbBox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
           labResult.Text = String.Format("日期1：{0}<br/>日期2：{1}<br/>",
                dp.Text,
                tbBox.Text
                );
        }
    }
}
