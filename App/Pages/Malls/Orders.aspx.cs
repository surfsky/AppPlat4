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
    [UI("订单管理")]
    [Auth(Powers.OrderView, Powers.OrderNew, Powers.OrderEdit, Powers.OrderDelete)]
    public partial class Orders : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("OrderForm.aspx")
                .InitGrid<Order>(BindGrid, Panel1, t => t.Summary, true)
                ;
            if (!IsPostBack)
            {
                UI.BindEnum(ddlType, typeof(ProductType), "--全部类别--");
                UI.BindEnum(ddlSts, typeof(OrderStatus), "--全部状态--");
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbMobile, this.tbSerialNo, this.tbName, this.ddlSts, this.dpStart, this.dpEnd);
                UI.SetValue(dpStart, DateTime.Today.AddMonths(-6));
                MallHelper.LimitShop(this.pbShop);

                this.Grid1.SetSortPage<Order>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
            }
        }


        // 绑定网格
        private void BindGrid()
        {
            IQueryable<Order> q = Query();
            Grid1.Bind(q);
        }
        private IQueryable<Order> Query()
        {
            var userId = Asp.GetQueryLong("userId");
            var sts = UI.GetInt(ddlSts);
            var shopId = UI.GetLong(pbShop);
            var type = UI.GetEnum<ProductType>(ddlType);
            var mobile = UI.GetText(tbMobile);
            var serialNo = UI.GetText(tbSerialNo).Trim();
            var name = UI.GetText(tbName).Trim();
            var startDt = UI.GetDate(dpStart);
            var endDt = UI.GetDate(dpEnd);
            IQueryable<Order> q = Order.Search(
                status: sts,
                userName: name,
                startDt: startDt,
                endDt: endDt,
                userId: userId,
                userMobile: mobile,
                serialNo: serialNo,
                shopId: shopId,
                type: type
                );
            return q;
        }


        // 关闭本窗口
        protected void btnClose_Click(object sender, EventArgs e)
        {
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        // 导出excel
        protected void Grid1_Export(object sender, EventArgs e)
        {
            var list = Query().ToList().Select( t => new
            {
                类型 = t.TypeName,
                状态 = t.StatusName,
                创建商店 = t.Shop?.AbbrName,
                受理商店 = t.HandleShop?.AbbrName,
                创建时间 = t.CreateDt,
                用户 = t.User?.NickName,
                过期时间 = t.ExpireDt,
                序列号 = t.SerialNo,
                概述 = t.Summary,
                数量 = t.TotalAmount,
                金额 = t.TotalMoney,
                支付时间 = t.PayDt,
                支付类型 = t.PayModeName,
                支付金额 = t.PayMoney,
                备注 = t.Remark
            }).ToList();
            this.Grid1.ExportExcel(list, "订单.xls");
        }
    }
}