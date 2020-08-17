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
    [UI("用户邀请")]
    [Auth(Powers.InviteView, Powers.InviteNew, Powers.InviteEdit, Powers.InviteDelete)]
    public partial class InviteForm : FormPage<Invite>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.BindEnum(ddlSource, typeof(InviteSource));
                UI.BindEnum(ddlStatus, typeof(InviteStatus));
                ShowForm();
            }
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------

        // 清空数据
        public override void NewData()
        {
            this.lblId.Text = "-1";
            UI.SetValue(this.ddlSource, null);
            UI.SetValue(this.ddlStatus, null);
            UI.SetValue(this.pbShop, Common.LoginUser?.ShopID);
            UI.SetValue(this.pbInviter, Common.LoginUser?.ID);
            UI.SetValue(this.pbInvitee, null);
            UI.SetValue(tbInviteeMobile, "");
            UI.SetValue(this.dpCreate, DateTime.Now);
            UI.SetValue(this.dpRegist, null);
            UI.SetValue(tbRemark, "");
        }

        // 加载数据
        public override void ShowData(Invite item)
        {
            this.lblId.Text = item.ID.ToString();
            UI.SetValue(this.ddlSource, item.Source);
            UI.SetValue(this.ddlStatus, item.Status);
            UI.SetValue(this.pbShop, item.InviteShopID);
            UI.SetValue(this.pbInviter, item.InviterID);
            UI.SetValue(this.pbInvitee, item.InviteeID);
            UI.SetValue(tbInviteeMobile, item.InviteeMobile);
            UI.SetValue(this.dpCreate, item.CreateDt);
            UI.SetValue(this.dpRegist, item.RegistDt);
            UI.SetValue(tbRemark, item.Remark);
        }

        // 采集数据
        public override void CollectData(ref Invite item)
        {
            item.Source = UI.GetEnum<InviteSource>(this.ddlSource);
            item.Status = UI.GetEnum<InviteStatus>(this.ddlStatus);
            item.InviteShopID = UI.GetLong(this.pbShop);
            item.InviterID = UI.GetLong(this.pbInviter);
            item.InviteeID = UI.GetLong(this.pbInvitee);
            item.InviteeMobile = UI.GetText(tbInviteeMobile);
            item.CreateDt = UI.GetDate(this.dpCreate);
            item.RegistDt =  UI.GetDate(this.dpRegist);
            item.Remark = UI.GetText(tbRemark);
        }
    }
}
