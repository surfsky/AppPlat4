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
    [UI("订单项资产")]
    [Auth(Powers.OrderView, Powers.OrderEdit, Powers.OrderEdit, Powers.OrderEdit)]
    public partial class OrderItemAssetForm : FormPage<OrderItemAsset>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                pbAsset.UrlTemplate = string.Format("userAssets.aspx?userId={0}", Asp.GetQueryLong("userId"));
                ShowForm();
            }
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------

        // 清空数据
        public override void NewData()
        {
            // userId
            var userId = Asp.GetQueryLong("userId");
            if (userId == null)
            {
                Asp.Fail("缺少 userId 参数");
                return;
            }
            // orderItemId
            var orderItemId = Asp.GetQueryLong("orderItemId");
            if (orderItemId == null)
            {
                Asp.Fail("缺少 orderItemId 参数");
                return;
            }

            var user = DAL.User.Get(userId);
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.lblCreateDt, "");
            UI.SetValue(this.lblUser, user.NickName);
            UI.SetValue(this.lblOrderItemID, orderItemId);
        }

        // 加载数据
        public override void ShowData(OrderItemAsset item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.lblCreateDt, item.CreateDt);
            UI.SetValue(this.lblUser, item.User.NickName);
            UI.SetValue(this.lblOrderItemID, item.OrderItemID);
            UI.SetValue(this.pbAsset, item.Asset, t => t.ID, t => t.Name);
        }

        // 采集数据
        public override void CollectData(ref OrderItemAsset item)
        {
            if (this.Mode == PageMode.New)
            {
                item.UserID = Asp.GetQueryLong("userId");
                item.OrderItemID = Asp.GetQueryLong("orderItemId");
            }
            item.AssetID = UI.GetLong(this.pbAsset);
        }
    }
}
