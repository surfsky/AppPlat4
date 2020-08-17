using System;
using System.Collections.Generic;
using System.Web;
using App.Components;
using App.Wechats;
using App.Utils;
using App.DAL;
using App.Wechats.OP;
using App.Controls;

namespace App.WeiXin
{
    /// <summary>
    /// 微信公众号消息处理
    /// 负责推送签名校验(GET) 及推送消息处理(POST XML)
    /// https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421135319
    /// https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140454
    /// 公众平台官网的开发-基本设置页面，勾选协议成为开发者，点击“修改配置”按钮，填写
    /// - URL是开发者用来接收微信消息和事件的接口URL。
    /// - Token可由开发者可以任意填写，用作生成签名（该Token会和接口URL中包含的Token进行比对，从而验证安全性）。
    /// - EncodingAESKey由开发者手动填写或随机生成，将用作消息体加解密密钥。
    /// 该URL负责处理
    /// （1）微信服务器将发送GET请求到填写的服务器地址URL上，做签名校验。此时必须认证通过。
    /// （2）当普通微信用户向公众账号发消息时，微信服务器将POST消息的XML数据包到开发者填写的URL上，服务器可解析这些信息并做处理
    /// 示例
    /// https://www.bearmanager.cn/WeiXin/WEBService.ashx?signature=a7dc992b9be9cf583c325753e86b180029be3192&echostr=7154795684745412365&timestamp=1554724621&nonce=1885473029
    /// </summary>
    [UI("微信公众号消息处理器", Remark = "使用微信自带鉴权机制")]
    [Auth(Ignore = true)]
    public class OPService : IHttpHandler
    {
        public bool IsReusable { get { return false; } }
        public void ProcessRequest(HttpContext context)
        {
            var response = context.Response;
            var q = context.Request.QueryString;
            var post = HttpHelper.GetPostText(context.Request);
            var info = string.Format("{0}\r\n{1}", post, Asp.BuildRequestHtml());
            Logger.LogDb("WechatOpen", post, "", LogLevel.Info);

            // 接入认证
            if (context.Request.HttpMethod == "GET")
            {
                var token = Wechats.WechatConfig.OPPushToken;
                var signature = q["signature"];
                if (signature == Wechat.CalcPushMessageSign(q["timestamp"], q["nonce"], token))
                    response.Write(q["echostr"]);   // 原样返回echostr参数内容，则接入生效
                else
                    response.Write("Fail to access : " + q["echostr"]);
                response.End();
                return;
            }

            // 微信公众号 POST 消息处理（请在5秒钟内及时处理）
            try
            {
                // 解析消息并获取用户详细信息
                // 如有需要创建用户，并修改用户微信相关信息
                var msg = post.ParseXml<PushMessage>();
                var openId = msg.FromUserName;
                var wechatUser = WechatOP.GetUserInfo(openId);
                User user = User.GetOrCreateWechatUser(wechatUser);
                user = user.EditByWechat(wechatUser);
                info = new { Msg = msg, WechatUser = wechatUser, User=user.Export() }.ToJson();
                Logger.LogDb("WechatOpen-Msg", info, wechatUser.nickname, LogLevel.Info);

                // 处理消息
                ProcessPost(msg, wechatUser, user);
            }
            catch (Exception ex)
            {
                Logger.LogWebRequest("WechatOpen-Process", ex, "");
                HttpContext.Current.Response.Write("");
            }
        }

        //-------------------------------------------------
        // 处理消息
        //-------------------------------------------------
        // 处理Post上来的消息类型信息
        void ProcessPost(PushMessage msg, WechatUser wechatUser, User user)
        {
            if (msg.MsgType ==  PushMessageType.Text)                ProcessTextMsg(msg, wechatUser, user);
            else if (msg.MsgType == PushMessageType.Voice)           ProcessVoiceMsg(msg, wechatUser, user);
            else if (msg.MsgType == PushMessageType.Event)
            {
                if      (msg.Event == PushEventType.Subscribe)   ProcessSubscribeEvent(msg, wechatUser, user);
                else if (msg.Event == PushEventType.Unsubscribe) ProcessUnsubscribeEvent(msg, wechatUser, user);
                else if (msg.Event == PushEventType.Scan)        ProcessScanEvent(msg, wechatUser, user);
                else if (msg.Event == PushEventType.Location)    ProcessLocationEvent(msg, wechatUser, user);
                else if (msg.Event == PushEventType.View)        ProcessViewEvent(msg, wechatUser, user);
                else if (msg.Event == PushEventType.Click)       ProcessClickEvent(msg, wechatUser, user);
            }
            HttpContext.Current.Response.Write("");
        }

        //---------------------------------------------
        // 处理菜单点击事件
        //---------------------------------------------
        private void ProcessClickEvent(PushMessage msg, WechatUser wechatUser, User user)
        {
            // 商务联系
            if (msg.EventKey == "bussiness")
            {
                var txt = msg.ReplyText("请微信联系：YDTEA333。\r\n请备注：小熊管家门店加盟");
                HttpContext.Current.Response.Write(txt);
            }
        }

        //---------------------------------------------
        // 处理文本及语音消息
        //---------------------------------------------
        // 处理文本消息
        private void ProcessTextMsg(PushMessage msg, WechatUser wechatUser, User user)
        {
            string reply = ProcessText(msg, msg.Content);
            Logger.LogDb("WechatOpen-TextMsg", reply, wechatUser.nickname, LogLevel.Info);
        }

        // 处理语音消息（需要在公众号后台开启声音识别能力）
        private void ProcessVoiceMsg(PushMessage msg, WechatUser wechatUser, User user)
        {
            string reply = ProcessText(msg, msg.Recognition);
            Logger.LogDb("WechatOpen-VoiceMsg", reply, wechatUser.nickname, LogLevel.Info);
        }

        /// <summary>处理用户文本指令</summary>
        /// <remarks>可参考实现回复规则: https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1433751299</remarks>
        private static string ProcessText(PushMessage msg, string text)
        {
            var reply = "";
            if (text.Contains("你好"))
            {
                reply = msg.ReplyText("你好，我是小熊 0_o");
            }
            else if (text.Contains("手机维修"))
            {
                reply = msg.ReplyText("请点击菜单：自助服务>维修下单");
                //reply = msg.ReplyMiniProgram(WechatConfig.mpAppID, "pages/index/index");
            }
            else
            {
                reply = msg.ReplyText("欢迎关注小熊手机管家 >_<");
            }
            HttpContext.Current.Response.Write(reply);
            return reply;
        }




        //---------------------------------------------
        // 处理订阅事件
        //---------------------------------------------
        // 处理取消订阅事件
        private void ProcessUnsubscribeEvent(PushMessage msg, WechatUser wechatUser, User user)
        {
            // 处理订阅及邀请信息
            user.WechatOPSubscribe = false;
            user.Save();
        }

        // 处理订阅事件
        private void ProcessSubscribeEvent(PushMessage msg, WechatUser wechatUser, User user)
        {
            // 解析场景参数
            var scene = "";
            var key = "qrscene_";
            if (msg.EventKey.IsNotEmpty() && msg.EventKey.StartsWith(key))
                scene = msg.EventKey.Substring(key.Length);
            var o = new { key = msg.EventKey, scene = scene };
            Logger.LogDb("WechatOpen-Subscribe", o.ToJson(), wechatUser.nickname, LogLevel.Info);

            // 处理订阅及邀请信息
            user.WechatOPSubscribe = true;
            user.Save();
            ProcessInvite(wechatUser, user, scene);
        }


        // 处理用户扫描事件（已订阅用户扫描二维码会触发该消息；未订阅用户扫描二维码只会触发subscribe消息）
        private void ProcessScanEvent(PushMessage msg, WechatUser wechatUser, User user)
        {
            var key = msg.EventKey;
            Logger.LogDb("WechatOpen-Scan", "scan event. key=" + key, wechatUser.nickname, LogLevel.Info);
            ProcessInvite(wechatUser, user, key);
        }


        // 邀请处理
        private static void ProcessInvite(WechatUser wechatUser, User user, string inviteUrl)
        {
            if (inviteUrl.IsEmpty())
                return;

            // 解析邀请门店及用户
            Url url = new Url(inviteUrl);
            var inviteShopId = url["inviteShopId"]?.ParseInt();
            var inviteUserId = url["inviteUserId"]?.ParseInt();
            //Logger.LogDb("WechatOpen-ProcessInvite", new { inviteShopId = inviteShopId, inviteUserId = inviteUserId, user = user?.Export(false), url = inviteUrl }.ToJson(), wechatUser.nickname, LogLevel.Info);

            // 处理邀请及奖励
            var invite = Logic.InviteAndAward(user, inviteUserId, inviteShopId);
            //Logger.LogDb("WechatOpen-InviteAndAward", invite?.ExportJson(false), wechatUser.nickname, LogLevel.Info);
        }




        //-------------------------------------------------
        // 位置信息事件
        //-------------------------------------------------
        private void ProcessLocationEvent(PushMessage msg, WechatUser wechatUser, User user)
        {
            user.LastGPS = string.Format("{0},{1}", msg.Longitude, msg.Latitude);
            user.Save();
            var o = new { type = "location event", x = msg.Longitude, y = msg.Latitude };
            Logger.LogDb("WechatOpen-Location", o.ToJson(), wechatUser.nickname, LogLevel.Info);
        }

        //-------------------------------------------------
        // 页面查看事件
        //-------------------------------------------------
        private void ProcessViewEvent(PushMessage msg, WechatUser wechatUser, User user)
        {
        }


    }
}