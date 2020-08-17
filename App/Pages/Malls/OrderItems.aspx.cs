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
    [UI("订单细项目管理")]
    [Auth(Powers.OrderView, Powers.OrderNew, Powers.OrderEdit, Powers.OrderDelete)]
    [Param("orderId", "订单ID")]
    public partial class OrderItems : PageBase
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
                .SetUrls(
                    "OrderItemForm.aspx?md=new&orderId=" + orderId,
                    "OrderItemForm.aspx?md=view&orderId=" + orderId,
                    "OrderItemForm.aspx?md=edit&id={0}&orderId=" + orderId)
                .InitGrid<OrderItem>(BindGrid, Panel1, t => t.Title)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<OrderItem>(SiteConfig.Instance.PageSize, t => t.ID, true);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbTitle, this.btnSearch);
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            string title = tbTitle.Text.Trim();
            var orderId = Asp.GetQueryLong("orderId");
            if (orderId == null)
                return;
            IQueryable<OrderItem> q = OrderItem.Search(orderId, title, null);
            Grid1.Bind(q);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}