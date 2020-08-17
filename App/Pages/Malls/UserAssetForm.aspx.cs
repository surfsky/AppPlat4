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
    [UI("用户资产")]
    [Auth(Powers.AssetView, Powers.AssetNew, Powers.AssetEdit, Powers.AssetDelete)]
    [Param("userId", "用户ID")]
    public partial class UserAssetForm : FormPage<UserAsset>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                this.pbUser.Enabled = false;       // 用户也不允许手工修改，通过参数带入
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
            UI.SetValue(this.pbUser, user, t => t.ID, t => t.NickName);


            //
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.dpInsuranceStart, DateTime.Now.TrimDay());
            UI.SetValue(this.dpInsuranceEnd, DateTime.Now.TrimDay().AddYears(1));
            UI.SetValue(this.tbName, "");
            UI.SetValue(this.tbSerialNo, "");
        }

        // 加载数据
        public override void ShowData(UserAsset item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.pbUser, item.User, t => t.ID, t => t.NickName);
            UI.SetValue(this.dpInsuranceStart, item.InsuranceStartDt);
            UI.SetValue(this.dpInsuranceEnd, item.InsuranceEndDt);
            UI.SetValue(this.tbName, item.Name);
            UI.SetValue(this.tbSerialNo, item.SerialNo);
        }

        // 采集数据
        public override void CollectData(ref UserAsset item)
        {
            item.UserID = UI.GetLong(this.pbUser);
            item.InsuranceStartDt = UI.GetDate(this.dpInsuranceStart);
            item.InsuranceEndDt = UI.GetDate(this.dpInsuranceEnd);
            item.Name = UI.GetText(this.tbName);
            item.SerialNo = UI.GetText(this.tbSerialNo);
        }
    }
}
