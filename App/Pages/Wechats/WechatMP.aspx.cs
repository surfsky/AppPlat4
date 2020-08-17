using System;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Wechats;
using App.Wechats.MP;
using App.Components;

namespace App.Tests
{
    [UI("微信小程序测试")]
    [Auth(Powers.Admin)]
    public partial class TestWechatMP : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.tbTemplateId.Text = "ByW6hs7EpOnmD23XGpHrZwzGdCOu3VOcHOrVFAItU7w";
            }
        }

        // 显示反馈的错误码
        void ShowReply(WechatReply reply)
        {
            lblErrCode.Text = reply?.errcode.ToText();
            lblErrInfo.Text = reply?.errmsg;
        }


        // 获取Session
        protected void btnGetSession_Click(object sender, EventArgs e)
        {
            var code = UI.GetText(tbCode);
            var o = WechatMP.JSCode2Session(code);
            this.tbOpenId.Text = o.openid;
            this.tbUnionId.Text = o.unionid;
            this.tbSession.Text = o.session_key;
            this.photo.ImageUrl = "";
            ShowReply(o);
        }


        // 获取用户详细信息
        protected void btnGetUserInfo_Click(object sender, EventArgs e)
        {
            var sessionKey = UI.GetText(tbSession); // "TzOQTtV5kjgc4ROeaU7kuQ==";
            var reply = UI.GetText(tbUserInfo);
            if (reply.IsEmpty() || sessionKey.IsEmpty())
            {
                UI.SetInvalid(tbUserInfo, "wx.getUserInfo() 返回文本");
                UI.SetText(lblInfo, "请输入getUserInfo  和 sessionKey 信息");
                return;
            }
            var user = WechatMP.DecryptUserInfo(reply, sessionKey);
            UI.ShowAlert(user.ToJson());
        }

        // 获取后台API访问Token
        protected void btnGetToken_Click(object sender, EventArgs e)
        {
            tbToken.Text = WechatMP.GetAccessTokenFromServer();
        }

        // 刷新token
        protected void btnRefreshToken_Click(object sender, EventArgs e)
        {
            tbToken.Text = WechatMP.GetAccessTokenFromServer(true);
        }

        // 发送小程序模板消息
        protected void btnSendMsg_Click(object sender, EventArgs e)
        {
            var openId = UI.GetText(tbOpenId);
            var formId = UI.GetText(tbFormId);
            if (openId.IsEmpty() || formId.IsEmpty())
            {
                lblInfo.Text = "请输入微信小程序OpenId 和 FormID";
                return;
            }
            var templateId = UI.GetText(tbTemplateId);
            var page = UI.GetText(tbPage);
            var data = new TMessage(
                templateId, page,
                "", "", 
                UI.GetText(tbKeyword1), 
                UI.GetText(tbKeyword2), 
                UI.GetText(tbKeyword3),
                UI.GetText(tbKeyword4), 
                UI.GetText(tbKeyword5)
                );
            var reply = WechatMP.SendTMessage(openId, data, formId);
            ShowReply(reply);
        }

        // 获取二维码
        protected void btnGetQrCode_Click(object sender, EventArgs e)
        {
            var path = UI.GetText(tbPath).UrlEncode();
            var width = UI.GetInt(tbSize);
            if (width != null)
            {
                var url = string.Format("/HttpApi/Wechat/MPQrCode?page={0}&width={1}&t={2}", path, width, DateTime.Now.Ticks);
                this.imgQrCode.ImageUrl = url;
            }
        }

        // 同时发送微信公众号和微信小程序消息
        protected void btnSendMsg2_Click(object sender, EventArgs e)
        {
            var user = DAL.User.Get(UI.GetLong(pbUser));
            var order = DAL.Order.Get(UI.GetLong(pbOrder));
            if (user != null && order != null)
            {
                var reply = Logic.SendWechatOrderMessage(order);
                UI.ShowAlert(reply.ToJson());
            }
        }

    }
}
