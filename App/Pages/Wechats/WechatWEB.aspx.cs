using System;
using System.Web.UI;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Wechats;
using App.Wechats.MP;
using App.Wechats.OP;
using App.Components;

namespace App.Tests
{
    [UI("微信公众号测试")]
    [Auth(Powers.Admin)]
    public partial class TestWechatWEB : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.tbTemplateId.Text = "KAXUYy48a4rDWbyY0kgAuQkYWCsSRfm09AprzexHYhE";
            }
        }

        // 显示反馈的错误码
        void ShowReply(WechatReply reply)
        {
            lblErrCode.Text = reply?.errcode.ToText();
            lblErrInfo.Text = reply?.errmsg;
            UI.ShowAlert(reply.ToJson());
        }


        // 获取Session
        protected void btnGetSession_Click(object sender, EventArgs e)
        {
            var code = UI.GetText(tbCode);
            var o = WechatOP.OAuthGetUserInfo(code);
            this.tbOpenId.Text = o.openid;
            this.tbUnionId.Text = o.unionid;
            this.photo.ImageUrl = o.headimgurl;
            ShowReply(o);
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

        // 发送模板消息
        protected void btnSendMsg_Click(object sender, EventArgs e)
        {
            var templateId = UI.GetText(tbTemplateId);
            var openId = UI.GetText(tbOpenId);
            if (openId.IsEmpty() || templateId.IsEmpty())
            {
                lblInfo.Text = "请输入微信公众号 OpenId 和微信公众号模板消息ID";
                return;
            }
            var page = UI.GetText(tbPage);
            var data = new TMessage(
                templateId, page,
                UI.GetText(tbFirst), 
                UI.GetText(tbRemark), 
                UI.GetText(tbKeyword1), 
                UI.GetText(tbKeyword2), 
                UI.GetText(tbKeyword3), 
                UI.GetText(tbKeyword4), 
                UI.GetText(tbKeyword5)
                );
            var reply = WechatOP.SendTMessage(openId, data);
            ShowReply(reply);
        }

        // 获取二维码
        protected void btnGetQrCode_Click(object sender, EventArgs e)
        {
            var path = UI.GetText(tbPath).UrlEncode();
            var url = string.Format("/HttpApi/Wechat/WEBQrCode?page={0}&t={1}", path, DateTime.Now.Ticks);
            this.imgQrCode.ImageUrl = url;
        }
    }
}
