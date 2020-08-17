using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Controls;
using App.Components;

namespace App.Pages
{
    [UI("订单处理")]
    [Auth(Powers.OrderEdit)]
    [Param("orderId", "订单ID")]
    [Param("statusId", "状态ID")]
    [Param("statusName", "状态名", false)]
    public partial class OrderProcess : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Common.CheckPower(this.Auth.ViewPower);
            if (!IsPostBack)
                ShowForm();
        }

        // 加载数据
        public void ShowForm()
        {
            var orderId = Asp.GetQueryLong("orderId");
            var statusId = Asp.GetQueryLong("statusId");
            var action = Asp.GetQueryString("action");
            var statusName = Asp.GetQueryString("statusName");
            if (orderId == null || statusId == null)
                Asp.Fail("请输入 orderId 和 statusId 参数");

            // 显示订单及要操作的步骤信息
            var order = Order.Get(orderId);
            var info = string.Format("{0}({1})", order.Summary, order.SerialNo);
            UI.SetValue(lblOrder, info);
            UI.SetValue(lblAction, action);
            UI.SetValue(lblStatus, statusName);

            // 订单特殊环节处理
            if (order.Type == ProductType.Repair)
            {
                if (statusId == (int)RepairStatus.BizSend || statusId == (int)RepairStatus.SendRepairOK)
                {
                    tbRemark.EmptyText = "快递单号";
                    tbRemark.Required = true;
                    tbRemark.ShowRedStar = true;
                }
            }
        }

        // 保存
        protected void btnOk_Click(object sender, EventArgs e)
        {
            var orderId = Asp.GetQueryLong("orderId");
            var statusId = Asp.GetQueryLong("statusId");
            var action = Asp.GetQueryString("action");
            var statusName = Asp.GetQueryString("statusName");
            var order = Order.Get(orderId);
            var remark = UI.GetText(tbRemark);
            var pics = new List<string>();
            var pic1 = UI.GetUrl(img1);
            var pic2 = UI.GetUrl(img2);
            var pic3 = UI.GetUrl(img3);
            if (pic1.IsNotEmpty()) pics.Add(pic1);
            if (pic2.IsNotEmpty()) pics.Add(pic2);
            if (pic3.IsNotEmpty()) pics.Add(pic3);

            try
            {
                order.ChangeStatus(statusName, (int)statusId.Value, Common.LoginUser.ID, remark, pics);

                // 维修单特殊处理
                if (order.Type == ProductType.Repair && statusId == (int)RepairStatus.SendRepairOK)
                {
                    order.Remark += remark;
                    order.Save();
                }
                UI.HideWindow(CloseAction.HideRefresh);
            }
            catch(Exception ex)
            {
                UI.ShowAlert(ex.Message);
            }
        }

        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            // controls
            var uploader = sender as FineUIPro.FileUpload;
            Thrumbnail img = null;
            if (uploader == uploader1) img = img1;
            if (uploader == uploader2) img = img2;
            if (uploader == uploader3) img = img3;

            // img
            var imageUrl = UI.UploadFile(uploader, "Histories", SiteConfig.Instance.SizeMiddleImage);
            UI.SetValue(img, imageUrl, true);
        }
    }
}
