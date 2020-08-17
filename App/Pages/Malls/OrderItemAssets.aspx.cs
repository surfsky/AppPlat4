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
    [UI("订单细项目")]
    [Auth(Powers.AssetView, Powers.AssetNew, Powers.AssetEdit, Powers.AssetDelete)]
    [Param("userId", "用户ID")]
    [Param("orderItemId", "订单细项ID")]
    public partial class OrderItemAssets : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var orderItemId = Asp.GetQueryLong("orderItemId");
            var userId = Asp.GetQueryLong("userId") ?? OrderItem.GetDetail(orderItemId.Value).Order.UserID;
            var newUrl = string.Format("OrderItemAssetForm.aspx?md=new&userId={0}&orderItemId={1}", userId, orderItemId);
            this.Grid1.New += Grid1_New;
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    newUrl,
                    "OrderItemAssetForm.aspx?md=view&id={0}",
                    "OrderItemAssetForm.aspx?md=edit&id={0}")
                .InitGrid<DAL.UserAsset>(BindGrid, Panel1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<OrderItemAsset>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbUser, this.btnSearch);
            }
        }

        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        /// <summary>显示数据</summary>
        private void BindGrid()
        {
            var userId = Asp.GetQueryLong("userId");
            var orderItemId = Asp.GetQueryLong("orderItemId");
            var user = UI.GetText(tbUser);
            var serialNo = UI.GetText(tbSerialNo);
            IQueryable<OrderItemAsset> q = OrderItemAsset.Search(
                userId: userId,
                userName: user,
                serialNo: serialNo,
                orderItemId: orderItemId
                );
            Grid1.Bind(q);
        }


        /// <summary>新增事件（做新增校验）</summary>
        private void Grid1_New(object sender, string url)
        {
            try
            {
                // 如果有订单参数的话，做下订单校验
                var orderItemId = Asp.GetQueryLong("orderItemId");
                if (orderItemId != null)
                {
                    var orderItem = OrderItem.GetDetail(orderItemId.Value);
                    var assets = orderItem.GetAssets();
                    if (assets.Count >= orderItem.Amount)
                        throw new Exception("设备数目不允许超过商品数量");
                }
                this.Grid1.ShowWindow(url, Grid1.NewText);
            }
            catch (Exception ex)
            {
                UI.ShowAlert(ex.Message);
                Grid1.NewButton.Enabled = false;
            }
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
