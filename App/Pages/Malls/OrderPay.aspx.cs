using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Components;

namespace App.Pages
{
    [UI("订单支付")]
    [Auth(Powers.OrderEdit)]
    [Param("orderId", "订单ID")]
    public partial class OrderPay : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Common.CheckPower(this.Auth.ViewPower);
            if (!IsPostBack)
            {
                UI.BindEnum(ddlPayMode, typeof(OrderPayMode), null);
                ShowForm();
            }
        }

        // 加载数据
        public void ShowForm()
        {
            var orderId = Asp.GetQueryLong("orderId");
            if (orderId == null)
                Asp.Fail("请输入 orderId  参数");

            // 显示订单信息
            var item = Order.Get(orderId);
            tbTotalMoney.Readonly = !Common.LoginUser.HasPower(Powers.Admin);
            UI.SetValue(this.tbGUID, item.SerialNo);
            UI.SetValue(this.tbTotalMoney, item.TotalMoney);
            UI.SetValue(this.tbSummary, item.Summary);
            UI.SetValue(this.tbPayMoney, item.PayMoney);
            UI.SetValue(this.ddlPayMode, item.PayMode);
            UI.SetValue(this.lblPayDt, item.PayDt?.ToString("yyyy-MM-dd HH:mm"));
            UI.SetValue(this.lblStatus, item.StatusName);
        }


        // 保存
        protected void btnOk_Click(object sender, EventArgs e)
        {
            var orderId = Asp.GetQueryLong("orderId");
            var statusId = Asp.GetQueryInt("statusId");
            var action = Asp.GetQueryString("action");
            var statusName = Asp.GetQueryString("statusName");

            var item = Order.Get(orderId);
            item.TotalMoney = UI.GetDouble(this.tbTotalMoney, 0);
            item.PayMode = UI.GetEnum<OrderPayMode>(this.ddlPayMode);
            item.PayMoney = UI.GetDouble(this.tbPayMoney, 0.0);
            item.PayDt = DateTime.Now;
            item.Save();
            item.ChangeStatus(statusName, statusId.Value, Common.LoginUser.ID);
            UI.HideWindow(CloseAction.HideRefresh);
        }
    }
}
