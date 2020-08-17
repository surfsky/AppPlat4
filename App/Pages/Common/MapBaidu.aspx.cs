using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Components;

namespace App.Pages
{
    [UI("地图定位窗口(baidu)")]
    [Param("gps", "GPS坐标")]
    [Param("addr", "文本地址")]
    [Auth(AuthLogin =true)]
    public partial class MapBaidu : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var gps = Asp.GetQueryString("value");
                var addr = Asp.GetQueryString("addr");
                this.tbGPS.Text = gps;
                this.tbAddr.Text = addr;
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            var txt = string.Format("{0}", this.tbGPS.Text);
            var script = ActiveWindow.GetWriteBackValueReference(txt, txt) + ActiveWindow.GetHideReference();
            PageContext.RegisterStartupScript(script);
        }
    }
}
