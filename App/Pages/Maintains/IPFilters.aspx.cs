using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
//using EntityFramework.Extensions;
using App.DAL;
using App.Components;
using App.Controls;
using App.Utils;

namespace App.Admins
{
    [UI("IP黑名单管理")]
    [Auth(Powers.IPFilter)]
    public partial class IPFilters : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("IPFilterForm.aspx")
                .AddColumn<IPFilter>(t => t.IP, 200, "IP")
                .AddColumn<IPFilter>(t => t.StartDt, 200, "封禁时间")
                .AddColumn<IPFilter>(t => t.EndDt, 200, "解禁时间")
                .AddColumn<IPFilter>(t => t.Addr, 200, "地理位置")
                .AddColumn<IPFilter>(t => t.Remark, 200, "备注")
                .InitGrid<IPFilter>(this.BindGrid, Panel1, t => t.StartDt)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<IPFilter>(SiteConfig.Instance.PageSize, t => t.StartDt, true);
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            string name = ttbSearchMessage.Text.Trim();
            IQueryable<IPFilter> q = DAL.IPFilter.Search(name);
            Grid1.Bind(q);
        }

        // 查找
        protected void ttbSearchMessage_TriggerClick(object sender, string e)
        {
            BindGrid();
        }
    }
}
