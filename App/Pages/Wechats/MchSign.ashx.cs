using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using App.Components;
using App.Controls;
using App.Utils;
using App.HttpApi;
using App.Wechats;
using App.Wechats.MP;
using App.Wechats.Pay;

namespace App.WeiXin
{
    /// <summary>
    /// 请在微信商户平台设置（超级管理员账户）接入程序的ID和Key
    /// 已登录客户端可调用本接口，获取签名后的字符串。
    /// </summary>
    /// <remarks>https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_3</remarks>
    [UI("微信签名算法（使用商户Key）")]
    [Auth(AuthLogin=true)]
    public class MchSign : HandlerBase
    {
        public override void Process(HttpContext context)
        {
            var mchKey = WechatConfig.MchKey;
            var dict = new Url(context.Request.RawUrl).Dict;
            var sign = WechatPay.BuildPaySign(dict, mchKey);
            context.Response.Write(sign);
        }
    }
}