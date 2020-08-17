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
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Admins
{
    [UI("签到管理")]
    [Auth(Powers.SignView, Powers.SignNew, Powers.SignEdit, Powers.SignDelete)]
    public partial class Signs : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("SignForm.aspx")
                .InitGrid<DAL.UserSign>(BindGrid, Panel1, t => t.SignDt)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<UserSign>(SiteConfig.Instance.PageSize, t => t.SignDt, false);
                BindGrid();
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbUser, this.dpStart);
            }
        }

        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        private void BindGrid()
        {
            var user = UI.GetText(tbUser);
            var startDt = UI.GetDate(this.dpStart);
            IQueryable<UserSign> q = UserSign.Search(userName:user, startDt: startDt);
            Grid1.Bind(q);
        }

        //-------------------------------------------------
        // 工具栏
        //-------------------------------------------------
        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
