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
    public partial class TestWindows : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
                this.lblInfo.Text = DateTime.Now.ToShortDateString();
        }

        protected void btnWin_Click(object sender, EventArgs e)
        {
            this.lblInfo.Text = "Click";
            this.win.IFrameUrl = "/Pages/Configs/themes.aspx";
            PageContext.RegisterStartupScript(this.win.GetShowReference());
        }
    }
}
