using System;
using System.Drawing;
using System.ComponentModel;
using App.Utils;
using App.HttpApi;
using App.DAL;
using App.Wechats;
using App.Wechats.MP;
using App.Wechats.OP;
using App.Components;

namespace App.Apis
{
    [Scope("Mall")]
    [Description("微信相关接口。小程序专用接口前缀为MP，公众号专用接口前缀为OP。")]
    public class ApiWechat
    {
        [HttpApi("微信解绑", AuthLogin = true)]
        public static APIResult Unbind()
        {
            var user = Common.LoginUser;
            user.WechatUnionID = "";
            user.WechatMPID = "";
            user.WechatOPID = "";
            user.Save();
            Common.Logout();
            return new APIResult(true, "微信解绑成功");
        }

        //------------------------------------------------
        // 中控服务器接口
        //------------------------------------------------
        [HttpApi("获取后台访问token", AuthToken =true)]
        public static string GetAccessToken(WechatAppType type, bool refresh=false)
        {
            var host = Asp.Request.Url.Host.ToLower();
            var domain = SiteConfig.Instance.Domain.ToLower();
            if (host != domain)
                Logger.LogWebRequest("GetAccessToken");
            return Wechat.GetAccessToken(type, refresh);
        }


        //------------------------------------------------
        // 微信小程序接口
        //------------------------------------------------
        [HttpApi("微信小程序：登录凭证校验")]
        public static string MPJSCode2Session(string code)
        {
            var s = WechatMP.JSCode2Session(code);
            var o = new { code = code, session = s, aspSessionId = Asp.Session.SessionID };
            Logger.LogDb("WechatMPSession", o.ToJson(), s.openid, LogLevel.Info);

            // 尝试找到或创建微信用户，并修改其SessionKey属性
            if (s.session_key.IsNotEmpty())
            {
                User user = User.Get(wechatUnionId: s.unionid, wechatMPId: s.openid);
                if (user == null)
                    user = User.CreateCustomer(null);
                user.WechatUnionID = s.unionid;
                user.WechatMPID = s.openid;
                user.WechatMPSessionKey = s.session_key;
                user.Save();
                Common.LoginSuccess(user, 24 * 360);
                LoginLog.Regist(user.ID, "微信小程序：登录凭证校验");
            }
            return s.ToJson();
        }



        /*
        Param[nickname] 梁益鑫
        Param[photo] https://wx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJmO5r4Cx9rO2SS3AR6bAnZKdtNU5TDXBk5tibjQBPhibWPMHxasUP9ba2cib7dibgicyP4M9y97pbuXxQ/132 
        Param[openId] oVSMv5UWp7M3BXqpILNoE7R_sFi8
        Param[unionId] osJsO0lU5eoL53GkrgYabSB7Rw_I
        Param[getUserInfoReply]	{"errMsg":"getUserInfo:ok","rawData":"{\"nickName\":\"梁益鑫\",\"gender\":1,\"language\":\"zh_CN\",\"city\":\"Wenzhou\",\"province\":\"Zhejiang\",\"country\":\"China\",\"avatarUrl\":\"https://wx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJmO5r4Cx9rO2SS3AR6bAnZKdtNU5TDXBk5tibjQBPhibWPMHxasUP9ba2cib7dibgicyP4M9y97pbuXxQ/132\"}","userInfo":{"nickName":"梁益鑫","gender":1,"language":"zh_CN","city":"Wenzhou","province":"Zhejiang","country":"China","avatarUrl":"https://wx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJmO5r4Cx9rO2SS3AR6bAnZKdtNU5TDXBk5tibjQBPhibWPMHxasUP9ba2cib7dibgicyP4M9y97pbuXxQ/132"},"signature":"0ee01ba21b01dd34a6b5bb7750e66659c319e259","encryptedData":"O9LXDmVHFNOp8taALcYj/VZGNmbBPTTHODPOpahzZNGAWlT9UbADyWAUMA+pvFGaEW6oReLm87ljL5gQmrEptXd3EfoKBsOfjpFuWjWQRmtFI+z6EUKUrVpTMTMCcEvc2LgQk+PbMTKu2RrPeazjiw8Dlvia5UlbLJ6udssfxzOPqyU1beA0FVqF1eomfJoljn7hdHs2eEaUlCQ1sjYGzi9us63u09385joTpCx6Izkh7WVylsmiUe78BPCVxL9itD8QnunvpXddnaZWkUTikU3aOKVnigsaz7OGRDBT/zpE0jNmw0iJvOO3v6haSCzlEOwk4MF/lRFxV/3NqrkB1+4zrfGh1Ztrp6m9chrN8Jj//76FUbrmR8R/iSyVxJV3oKa7RxqIW/Dub5I6yHOCSskz0gFiKVjWFTtnwJp8YaoEhJctNi47mmIIKqCvm2HUnp0OmQdtQcv7zlg1jYEfRP2ELYR0eva+pFijGxgz2lM6DkG4dhIKuruJuQoT08rrYM9PzI2R6wVkkNP70j06WY9tQdoaXdDkBQYloZbJ/2Y=","iv":"y48789PZv3xYHVkfaw6xBQ=="}
        Param[APPAUTH] FB3F8DEFAA075C42C85F9AB2C6E3ECC710AA18EDD2F1BD77EEEC8760FAFE2190BBD601A38B394A1055724B5F24F18D1E5568813E431BB7DA3293ED841F31ADB79DCA6D72DB615DF9112CE8199D8305A58EA04C583A868536511CBA5810B6EB4B2C6226B76EDB8388CF33A59B6E5F699910F6AA9D7DBFA82EB98F0645AE3CC91E84A7DF287607B593FE8AC86D0D56B562A87A7757D3587022FCA34660ACA6257AD978554984DBBC904351F955F3159A7D
        Param[ASP.NET_SessionId] gscvlqsqwspks0lv0lzdwrdg
        */
        [HttpApi("微信小程序：授权登陆（若用户不存在则创建）", Log=true)]
        [HttpParam("getUserInfoReply", "wx.getUserInfo() 的返回数据")]
        public static APIResult MPLogin(string unionId, string openId, string nickname, string photo, 
            long? inviteUserId = null, long? inviteShopId = null, string getUserInfoReply="")
        {
            var user = User.Get(wechatUnionId: unionId, wechatMPId: openId);
            var sessionKey = user.WechatMPSessionKey;
            var wechatUser = new WechatUser() { unionid = unionId, mpId = openId, nickname = nickname, headimgurl = photo, mpSessionKey = sessionKey };
            var txt = string.Format("user={0}, reply={1}, sessionKey={2}, aspSessionID={3}", wechatUser.ToJson(), getUserInfoReply, sessionKey, Asp.Session.SessionID);
            Logger.LogDb("WechatMPLogin-Start", txt, nickname, LogLevel.Info);

            // 更新微信用户信息（尝试获取 UnionID）
            if (unionId.IsEmpty() && getUserInfoReply.IsNotEmpty() && sessionKey.IsNotEmpty())
            {
                var info = WechatMP.DecryptUserInfo(getUserInfoReply, sessionKey);
                if (info != null)
                {
                    Logger.LogDb("DecrytUserInfo-Ok", info.ToJson(), "", LogLevel.Debug);
                    wechatUser.unionid = info.unionId;
                    if (wechatUser.headimgurl.IsEmpty())
                        wechatUser.headimgurl = info.avatarUrl;
                }
            }
            Logger.LogDb("WechatMPLogin-Ok", wechatUser.ToJson(), nickname, LogLevel.Info);

            // 登录
            user.EditByWechat(wechatUser);
            Logic.InviteAndAward(user, inviteUserId, inviteShopId);
            Common.LoginSuccess(user, 24 * 360); // 微信小程序一直保持登录状态，故设置长一点，一年吧
            LoginLog.Regist(user.ID, "微信小程序：授权登陆（若用户不存在则创建）");
            return Common.LoginUser.ToResult(ExportMode.Detail, "登录成功");
        }


        [HttpApi("微信小程序：添加FormID")]
        public static APIResult MPAddFormId(long userId, string formId, long? orderId = null)
        {
            var item = WechatMPForm.Create(userId, formId, orderId);
            return item.ToResult();
        }


        [HttpApi("微信小程序：二维码", cacheSeconds: 60)]
        [HttpParam("page", "微信小程序页面路径，如/index/index?inviteShopId=9&inviteUserId=1")]
        public static Image MPQrCode(string page, int width)
        {
            return WechatMP.GetQrCode(page, width);
        }


        [HttpApi("微信小程序：解密手机号码")]
        public static APIResult MPDecryptPhoneData(string data, string iv, string openId)
        {
            var user = User.Get(wechatMPId: openId);
            var sessionKey = user.WechatMPSessionKey;
            return WechatMP.DecryptPhoneData(data, iv, sessionKey).ToResult();
        }

        /*
        [HttpApi("微信小程序：二维码", cacheSeconds: 60)]
        public static Image MPQrCode2(long? inviteShopId, long? inviteUserId, string inviteUserMobile)
        {
            var page = string.Format("inviteShopId={0}&inviteUserId={1}&inviteUserMobile={2}", inviteShopId, inviteUserId, inviteUserMobile).ToUrlEncode();
            return WechatMP.GetQrCode(page, 80);
        }
        */

        //------------------------------------------------
        // 微信公众号接口
        //------------------------------------------------
        [HttpApi("微信公众号：获取JS-SDK凭证", Wrap = true)]
        public static APIResult OPGetJsSdkSignature(string url)
        {
            return WechatOP.GetJsSdkSignature(url).ToResult();
        }

        [HttpApi("微信公众号：获取 AccessToken")]
        public static object OPGetToken(string code)
        {
            return WechatOP.OAuthGetAccessToken(code);
        }

        
        //[HttpApi("微信公众号：授权登陆（若用户不存在则创建）")]
        public static APIResult OPLogin(string code, long? inviteShopId = null, long? inviteUserId = null)
        {
            try
            {
                var wechatUser = WechatOP.OAuthGetUserInfo(code);
                User user = User.GetOrCreateWechatUser(wechatUser);
                Logic.InviteAndAward(user, inviteUserId, inviteShopId);
                Common.LoginSuccess(user, 24 * 360); // 微信网站一直保持登录状态，故设置长一点，一年吧
                LoginLog.Regist(user.ID, "微信公众号：授权登陆（若用户不存在则创建）");
                return Common.LoginUser.ToResult(ExportMode.Detail, "登录成功");
            }
            catch (Exception e)
            {
                Logger.LogDb("WechatWebLoginFail", e.Message, code, LogLevel.Warn);
                return new APIResult(false, "登陆失败." + e.Message);
            }
        }

        [HttpApi("微信公众号：二维码", cacheSeconds: 60)]
        [HttpParam("page", "带参路径（需进行UrlEncode），如inviteShopId=1&inviteUserId=1")]
        public static Image OPQrCode(string page)
        {
            var url = WechatOP.CreateQrCode(page);
            var img = HttpHelper.GetNetworkImage(url);
            return Painter.DrawIcon(img, SiteConfig.Instance.LogoDark);
        }

    }
}