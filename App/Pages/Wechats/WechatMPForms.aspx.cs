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
    [UI("微信小程序表单ID管理")]
    [Auth(Powers.Admin)]
    public partial class WechatMPForms : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(typeof(WechatMPForm))
                .AddColumn<WechatMPForm>(t => t.CreateDt, 120)
                .AddColumn<WechatMPForm>(t => t.UserID, 120)
                .AddColumn<WechatMPForm>(t => t.OpenID, 120)
                .AddColumn<WechatMPForm>(t => t.UnionID, 120)
                .AddColumn<WechatMPForm>(t => t.FormID, 120)
                .AddColumn<WechatMPForm>(t => t.OrderID, 120)
                .AddColumn<WechatMPForm>(t => t.Times, 120)
                .InitGrid<WechatMPForm>(BindGrid, Grid1, t => t.FormID)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<WechatMPForm>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            IQueryable<WechatMPForm> q = DAL.WechatMPForm.Search(null);
            Grid1.Bind(q);
        }
    }
}
