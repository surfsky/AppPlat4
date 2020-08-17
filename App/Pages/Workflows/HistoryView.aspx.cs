using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Controls;
using App.Components;
using App.Entities;

namespace App.Pages
{
    [UI("操作历史视图")]
    [Auth(AuthLogin =true)]
    [Param("id", "ID")]
    public partial class HistoryView : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Asp.GetQueryLong("id");
                var history = History.Get(id.Value);
                ShowData(history);
            }
        }

        // 加载数据
        public void ShowData(History item)
        {
            UI.SetValue(this.lblStatus, item.Status);
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.lblCreateDt, item.CreateDt);
            UI.SetValue(this.lblUserName, item.UserName);
            UI.SetValue(this.lblUserMobile, item.UserMobile);

            this.panDetail.IFrameUrl = Urls.GetResesUrl(PageMode.View, item.UniID, "Histories", true);
        }
    }
}
