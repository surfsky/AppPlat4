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
using App.Apis;

namespace App.Pages
{
    [UI("用户注册")]
    [Auth(AuthLogin=true)]
    [Param("inviteCode", "邀请码", false)]
    public partial class Regist : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 解析邀请码
            string inviteCode = Asp.GetQueryString("inviteCode");
            this.tbInviteCode.Text = inviteCode;
            var data = QrCodeData.Parse(inviteCode);
            if (data != null)
            {
                this.tbInfo.Text = string.Format("{0}({1})", data.Title, data.Key);
            }
        }

        // 获取验证码
        protected void btnGetSmsCode_Click(object sender, EventArgs e)
        {
            var mobile = UI.GetText(this.tbMobile);
            var result = ApiCommon.SendSms(mobile, SmsType.Regist, AppType.Web);
            this.tbSmsCode.Text = "";
            UI.ShowAlert(result.Info);
        }

        // 注册
        protected void btnRegist_Click(object sender, EventArgs e)
        {
            var mobile = UI.GetText(this.tbMobile);
            var password = UI.GetText(this.tbPassword);
            var inviteCode = UI.GetText(this.tbInviteCode);
            var mobileCode = UI.GetText(this.tbSmsCode);

            // Todo: 微信端扫描显示该页面时，获取用户的昵称和性别
            var result = ApiUser.Regist(mobile, password, mobileCode, inviteCode);
            UI.ShowAlert(result.Info);
        }

    }
}
