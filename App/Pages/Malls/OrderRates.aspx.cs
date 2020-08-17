using App.Controls;
using App.DAL;
using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Utils;
using App.Components;

namespace App.Pages
{
    [UI("订单评价")]
    [Auth(Powers.RateView, Powers.RateNew, Powers.RateEdit, Powers.RateDelete)]
    [Param("orderId", "订单ID")]
    public partial class OrderRates : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            var orderId = Asp.GetQueryString("orderId");
            if (orderId.IsEmpty())
            {
                Asp.Fail("缺少 orderId 参数");
                return;
            }

            // Grid
            this.Grid1
                .SetPowers(this.Auth)
                .AddColumn<OrderRate>(t => t.User.NickName, 200, "用户")
                .AddColumn<OrderRate>(t => t.CreateDt, 200, "时间")
                .AddColumn<OrderRate>(t => t.Rate, 200, "评分")
                .AddColumn<OrderRate>(t => t.Comment, 200, "评论")
                .InitGrid<OrderRate>(this.BindGrid, Panel1, t => t.CreateDt)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<OrderRate>(SiteConfig.Instance.PageSize, t => t.CreateDt, true);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbTitle, this.btnSearch);
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var orderId = Asp.GetQueryLong("orderId");
            var shopId = Asp.GetQueryLong("shopId");
            if (orderId == null && shopId == null)
                return;
            IQueryable<OrderRate> q = OrderRate.Search(shopId, orderId);
            Grid1.Bind(q);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}