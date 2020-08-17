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
    /// <summary>
    /// 其实，本窗口不适合给客户编辑，应该是只读的。
    /// </summary>
    [UI("用户财务")]
    [Auth(Powers.FinanceView, Powers.FinanceNew, Powers.FinanceEdit, Powers.FinanceDelete)]
    [Param("userId", "用户ID")]
    [Param("orderId", "订单ID")]
    public partial class UserFinanceForm : FormPage<UserFinance>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.BindEnum(ddlType, typeof(FinanceType));
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
            var user = DAL.User.Get(userId);

            // type
            var orderId = Asp.GetQueryLong("orderId");

            //
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.lblCreateDt, "");
            UI.SetValue(this.pbUser, user, t => t.ID, t => t.NickName);
            UI.SetValue(this.ddlType, FinanceType.Consume);
            UI.SetValue(this.tbMoney, "");
            UI.SetValue(this.tbOrderId, orderId);
        }

        // 加载数据
        public override void ShowData(UserFinance item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.lblCreateDt, item.CreateDt);
            UI.SetValue(this.pbUser, item.User, t => t.ID, t => t.NickName);
            UI.SetValue(this.ddlType, item.Type);
            UI.SetValue(this.tbMoney, item.Money);
            UI.SetValue(this.tbOrderId, item.OrderID);
        }

        // 采集数据
        public override void CollectData(ref UserFinance item)
        {
            item.UserID = UI.GetLong(this.pbUser);
            item.Type = UI.GetEnum<FinanceType>(this.ddlType);
            item.Money = UI.GetDouble(this.tbMoney, 0);
            item.OrderID = UI.GetLong(this.tbOrderId, null);
        }

        // 保存数据
        public override void SaveData(UserFinance item)
        {
            item.Save();
            if (this.Mode == PageMode.New)
            {
                var user = DAL.User.Get(item.UserID);
                user.CalcFinance(item.Money.Value);
            }
        }
    }
}
