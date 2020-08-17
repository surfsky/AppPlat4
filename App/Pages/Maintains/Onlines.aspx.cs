using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using FineUIPro;
using App.DAL;
using App.Components;
using App.Controls;
using App.Utils;


namespace App.Admins
{
    [UI("在线用户列表")]
    [Auth(Powers.Online)]
    public partial class Onlines : PageBase
    {
        /*
        <f:WindowField WindowID="Window1" ToolTip="查看" HeaderText="用户" Width="100px" 
            DataTextField="User.NickName" SortField="User.NickName"
            DataIFrameUrlFields="User.ID"  DataIFrameUrlFormatString="UserForm.aspx?md=view&id={0}" 
            />
        <f:BoundField DataField="UpdateDt" SortField="UpdateDt" DataFormatString="{0:yyyy-MM-dd HH:mm}" Width="120px" HeaderText="最后操作时间" />
        <f:BoundField DataField="LoginDt" SortField="LoginDt" Width="120px" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderText="登录时间" />
        <f:BoundField DataField="User.RealName" SortField="User.RealName" Width="100px" HeaderText="姓名" />
        <f:BoundField DataField="IPAdddress" ExpandUnusedSpace="true" HeaderText="IP地址" />
        */ 
        protected void Page_Load(object sender, EventArgs e)
        {
            Grid1
                .SetPowers(true, false, false, false)
                .AddWindowColumn<Online>(t=>t.UserID, t=>t.User.NickName, "/pages/Base/userform.aspx?md=view&id={0}", 150, "用户")
                .AddColumn<Online>(t => t.UpdateDt, 150, "最后操作时间", "{0:yyyy-MM-dd HH:mm}")
                .AddColumn<Online>(t => t.IP, 120, "IP地址", "{0}")
                .InitGrid<Online>(BindGrid, Panel1, t=>t.UserID);
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<Online>(SiteConfig.Instance.PageSize, t => t.UpdateDt, true);
                BindGrid();
                UI.SetVisibleByQuery("search", this.ttbSearchMessage);
            }
        }

        // 显示
        private void BindGrid()
        {
            string name = ttbSearchMessage.Text.Trim();
            IQueryable<Online> q = Online.SearchOnlines(null, name);
            Grid1.Bind(q);
        }

        // 搜索
        protected void ttbSearchMessage_TriggerClick(object sender, string e)
        {
            BindGrid();
        }
    }
}
