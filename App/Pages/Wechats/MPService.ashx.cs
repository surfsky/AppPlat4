using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using App.Components;
using App.Controls;
using App.Utils;
using App.DAL;
using App.Wechats;

namespace App.WeiXin
{
    /// <summary>
    /// 微信小程序消息处理
    /// 负责推送签名校验(GET) 及推送消息处理(POST XML)
    /// 请在小程序官网的“设置-消息服务器”页面进行设置本URL、token等信息
    /// https://developers.weixin.qq.com/miniprogram/dev/api/custommsg/callback_help.html
    /// https://developers.weixin.qq.com/miniprogram/dev/framework/server-ability/message-push.html
    /// https://developers.weixin.qq.com/miniprogram/dev/api/custommsg/callback_help.html
    /// </summary>
    [UI("微信小程序消息处理器", Remark="使用微信自带鉴权机制")]
    [Auth(Ignore=true)]
    public class MPService : IHttpHandler
    {
        public bool IsReusable { get { return false; } }
        public void ProcessRequest(HttpContext context)
        {
            var q = HttpContext.Current.Request.QueryString;
            var response = context.Response;
            Logger.LogDb("WechatMPService", Asp.BuildRequestHtml(), "", LogLevel.Debug);

            // 接入认证
            if (context.Request.HttpMethod == "GET")
            {
                var token = Wechats.WechatConfig.MPPushToken;
                var signature = q["signature"];
                if (signature == Wechat.CalcPushMessageSign(q["timestamp"], q["nonce"], token))
                    response.Write(q["echostr"]);   // 原样返回echostr参数内容，则接入生效
                else
                    response.Write("Fail to access : " + q["echostr"]);
                response.End();
                return;
            }

            // 微信小程序消息处理
            var data = HttpHelper.GetPostText(context.Request);
        }
    }
}