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
    [UI("订单")]
    [Auth(Powers.OrderView, Powers.OrderNew, Powers.OrderEdit, Powers.OrderDelete)]
    public partial class OrderForm : FormPage<Order>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Form
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                var type = Asp.GetQuery<ProductType>("type");
                UI.BindEnum(ddlType, typeof(ProductType), "--请选择--", "", (long?)type);
                UI.BindEnum(ddlPayMode, typeof(OrderPayMode), null);
                ShowForm();
            }
        }

        // 显隐控件
        void ShowItems()
        {
            var type = UI.GetEnum<ProductType>(this.ddlType);
            var mode = this.Mode;
            var status = UI.GetInt(this.ddlStatus);

            // 根据商品类别显隐控件
            this.panAppt.Hidden = true;
            if (type == ProductType.Repair)
            {
                this.panAppt.Hidden = false;
            }

            // 根据订单状态显隐控件
            if (mode == PageMode.New)
            {
                // 新订单无需显示操作步骤和订单细项
                UI.SetVisible(false, this.ddlNextStep, this.btnOperate, this.btnSaveNew, this.panDetail, this.lblCreate, this.lblExpire);
            }
        }

        // 新增
        public override void NewData()
        {
            UI.SetValue(this.tbID, "-1");
            UI.SetValue(this.tbGUID, Order.BuildSerialNo());
            UI.SetValue(this.tbTotalMoney, "0");
            UI.SetValue(this.lblCreate, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            UI.SetValue(this.tbSummary, "");
            UI.SetValue(this.pbUser, Common.LoginUser, t => t.ID, t => t.NickName);
            //UI.SetValue(this.pbShop, Common.LoginUser.Shop, t => t.ID, t => t.AbbrName);
            MallHelper.LimitShop(this.pbShop);

            UI.SetValue(this.tbPayMoney, "0");
            UI.SetValue(this.ddlPayMode, null);
            UI.SetValue(this.lblPayDt, "");
            UI.SetValue(this.tbRemark, "");
            UI.SetValue(this.ddlStatus, OrderStatus.Create);
            ShowItems();
        }


        // 显示
        public override void ShowData(Order item)
        {
            pbUser.Readonly = true;
            pbShop.Readonly = true;
            tbTotalMoney.Readonly = !Common.LoginUser.HasPower(Powers.Admin);

            UI.SetValue(this.tbID, item.ID);
            UI.SetValue(this.ddlType, item.Type);
            UI.SetValue(this.tbGUID, item.SerialNo);
            UI.SetValue(this.tbTotalMoney, item.TotalMoney);
            UI.SetValue(this.lblCreate, item.CreateDt?.ToString("yyyy-MM-dd HH:mm:ss"));
            UI.SetValue(this.lblExpire, item.ExpireDt?.ToString("yyyy-MM-dd HH:mm:ss"));
            UI.SetValue(this.tbSummary, item.Summary);
            UI.SetValue(this.pbUser, item.User, t => t.ID, t => t.NickName);
            UI.SetValue(this.tbPayMoney, item.PayMoney);
            UI.SetValue(this.ddlPayMode, item.PayMode);
            UI.SetValue(this.lblPayDt, item.PayDt?.ToString("yyyy-MM-dd HH:mm"));
            UI.SetValue(this.pbShop, item.Shop, t => t.ID, t => t.AbbrName);
            UI.SetValue(this.pbHandleShop, item.HandleShop, t => t.ID, t => t.AbbrName);
            UI.SetValue(this.tbRemark, item.Remark);
            UI.SetValue(this.lblApptDevice, item.ApptDevice);
            UI.SetValue(this.lblApptDt, item.ApptDt);
            UI.SetValue(this.lblApptMobile, item.ApptMobile);
            UI.SetValue(this.lblApptUser, item.ApptUser);
            UI.SetValue(this.lblApptRemark, item.ApptRemark);


            // 显示当前状态和后继操作步骤
            UI.Bind(this.ddlStatus, item.GetSteps(), t => t.Status, t => t.StatusName);
            UI.SetValue(ddlStatus, item.Status);
            var steps = item.GetNextSteps(Common.LoginUser);
            UI.Bind(ddlNextStep, steps, t => t.Status, t => t.Action);
            if (steps.Count == 0)
            {
                this.ddlNextStep.Hidden = true;
                this.btnOperate.Hidden = true;
            }

            // 老订单不允许更改产品类别, 不允许更改状态
            this.ddlType.Readonly = true;
            this.ddlStatus.Readonly = true;
            this.panDetail.IFrameUrl = string.Format("OrderItems.aspx?orderId={0}&search=false", item.ID);
            this.panDetail.Hidden = false;
        }

        // 采集数据
        public override void CollectData(ref Order item)
        {
            item.Type          = UI.GetEnum<ProductType>(this.ddlType);
            item.ShopID       = UI.GetLong(this.pbShop);
            item.HandleShopID = UI.GetLong(this.pbHandleShop);
            item.SerialNo      = UI.GetText(this.tbGUID);
            item.Summary       = UI.GetText(this.tbSummary);
            item.TotalMoney    = UI.GetDouble(this.tbTotalMoney, 0);
            item.UserID        = UI.GetLong(this.pbUser);
            item.Status        = UI.GetInt(this.ddlStatus);
            item.StatusName    = UI.GetText(this.ddlStatus);
            item.Remark        = UI.GetText(this.tbRemark);

            // 支付相关（待定）
            item.PayMode       = UI.GetEnum<OrderPayMode>(this.ddlPayMode);
            item.PayMoney      = UI.GetDouble(this.tbPayMoney, 0.0);
        }


        //----------------------------------------------
        // 老订单处理环节
        //----------------------------------------------
        protected void btnOperate_Click(object sender, EventArgs e)
        {
            // 后继操作
            var nextStatusId   = UI.GetInt(ddlNextStep);
            if (nextStatusId == null)
            {
                UI.ShowAlert("请选择下步操作");
                return;
            }
            var order = this.GetData();
            var nextStep = order.GetFlow().GetStep(nextStatusId.Value);
            var action = nextStep.Action;
            var nextStatusName = nextStep.StatusName;

            // 通用步骤直接处理掉
            if (nextStatusId == (int)OrderStatus.UserPay)
            {
                var paymode = UI.GetEnum<OrderPayMode>(ddlPayMode);
                var payMoney = UI.GetDouble(tbPayMoney, 0);
                order = order.Pay(paymode, payMoney, "");
                this.ShowData(order);
            }
            else if (nextStatusId == (int)OrderStatus.Cancel)
            {
                order = order.Cancel();
                this.ShowData(order);
                UI.HideWindow(CloseAction.Hide);
            }
            else if (nextStatusId == (int)OrderStatus.Finish)
            {
                order = order.Finish();
                this.ShowData(order);
            }
            else if (nextStatusId == (int)OrderStatus.UserPay)
            {
                var url = string.Format("OrderPay.aspx?orderId={0}&statusId={1}&statusName={2}&action={3}", order.ID, nextStatusId, nextStatusName, action);
                UI.ShowWindow(this.win, url, "支付", 800, 500, CloseAction.HideRefresh);
            }
            // 维修订单定制
            else if (order.Type == ProductType.Repair)
            {
                var url = string.Format("OrderProcess.aspx?orderId={0}&statusId={1}&statusName={2}&action={3}", order.ID, nextStatusId, nextStatusName, action);
                UI.ShowWindow(this.win, url, "处理", 800, 700, CloseAction.HideRefresh);
            }
            else
            {
                // 直接改状态（不推荐）
                order.ChangeStatus(nextStatusName, nextStatusId.Value, null);
                this.ShowData(order);
            }

        }

        // 显示处理历史
        protected void btnHistory_Click(object sender, EventArgs e)
        {
            var order = this.GetData();
            if (order != null)
            {
                string url = Urls.GetHistoriesUrl(order.UniID, (int)order.Type);
                UI.ShowWindow(this.win, url, "处理历史", 800, 700, CloseAction.Hide);
            }
        }

    }
}
