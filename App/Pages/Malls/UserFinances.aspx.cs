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

namespace App.Pages
{
    [UI("用户财务记录管理")]
    [Auth(Powers.FinanceView, Powers.FinanceNew, Powers.FinanceEdit, Powers.FinanceDelete)]
    public partial class UserFinances : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string newUrl = string.Format("UserFinanceForm.aspx?md=new&userId={0}&orderId={1}", Asp.GetQueryLong("userId"), Asp.GetQueryLong("orderItemId"));
            this.Grid1
                .SetPowers(this.Auth)
                .SetTexts("新增财务记录", "删除财务记录")
                .SetUrls(
                    newUrl,
                    "UserFinanceForm.aspx?md=view&id={0}",
                    "UserFinanceForm.aspx?md=edit&id={0}")
                .InitGrid<DAL.UserFinance>(BindGrid, Panel1, t => t.CreateDt)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<UserFinance>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbUser, this.dpCreate, this.btnSearch);
            }
        }


        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        private void BindGrid()
        {
            var userId = Asp.GetQueryLong("userId");
            var orderId = Asp.GetQueryLong("orderId");
            var user = UI.GetText(tbUser);
            var createDt = UI.GetDate(this.dpCreate);
            IQueryable<UserFinance> q = UserFinance.Search(
                userId: userId,
                userName:user, 
                startDt: createDt,
                orderId: orderId
                );
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
