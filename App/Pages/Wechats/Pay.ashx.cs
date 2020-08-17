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
using App.Wechats.MP;
using App.Wechats.Pay;

namespace App.WeiXin
{
    /*
     <xml>
        <appid><![CDATA[wx0253f291b14e43dc]]></appid>
        <bank_type><![CDATA[CFT]]></bank_type>
        <cash_fee><![CDATA[1]]></cash_fee>
        <fee_type><![CDATA[CNY]]></fee_type>
        <is_subscribe><![CDATA[N]]></is_subscribe>
        <mch_id><![CDATA[1488073052]]></mch_id>
        <nonce_str><![CDATA[369f27e0301b43819d411eced5c22017]]></nonce_str>
        <openid><![CDATA[oVSMv5UHZRIykl6jtK_Al2DweYYY]]></openid>
        <out_trade_no><![CDATA[20190414000006]]></out_trade_no>
        <result_code><![CDATA[SUCCESS]]></result_code>
        <return_code><![CDATA[SUCCESS]]></return_code>
        <sign><![CDATA[1B16E4D341613734D5B7FA45CD07FB2D]]></sign>
        <time_end><![CDATA[20190414150718]]></time_end>
        <total_fee>1</total_fee>
        <trade_type><![CDATA[JSAPI]]></trade_type>
        <transaction_id><![CDATA[4200000291201904141325155795]]></transaction_id>
    </xml>
    */
    /// <summary>
    /// 支付回调
    /// 
    /// 此页面地址在【统一下单API】参数 notify_url 中设置
    /// 支付完成后，微信会把相关支付结果和用户信息发送给商户，商户需要接收处理，并返回应答。
    /// 同样的通知可能会多次发送给商户系统。商户系统必须能够正确处理重复的通知。
    /// 请在微信商户平台设置（超级管理员账户）接入程序的ID和Key
    /// 微信支付开发文档入口：https://pay.weixin.qq.com/wiki/doc/api/index.html
    /// 微信小程序支付回调：https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=9_7&index=8
    /// 微信 HTML 支付回调：https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_4
    /// </summary>
    [UI("微信支付回调处理器", Remark = "使用微信自带鉴权机制")]
    [Auth(Ignore = true)]
    public class Pay : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        // 处理请求
        public void ProcessRequest(HttpContext context)
        {
            // 原始 XML 消息及签名检测
            var xml = HttpHelper.GetPostText(context.Request);
            var mchKey = Wechats.WechatConfig.MchKey;
            Logger.LogDb("WechatPay", "微信支付回调: " + xml, "", LogLevel.Debug);
            if (!WechatPay.CheckPaySign(xml, mchKey))
            {
                Write(false, "签名校验错误");
                return;
            }

            // 将 XML 解析成对象并校验结果
            var reply = xml.ParseXml<PayCallback>();
            if (!reply.IsSuccess)
            {
                Write(true, "微信支付失败");
                return;
            }

            // 支付成功订单处理
            var info = ProcessOrder(reply.out_trade_no);
            if (reply.appid == Wechats.WechatConfig.MPAppId)
                FetchUserUnionId(reply.openid);
            Write(true, info);
        }


        /// <summary>输出</summary>
        void Write(bool success, string msg)
        {
            var o = new
            {
                return_code = (success) ? "SUCCESS" : "FAIL",
                return_msg = msg
            };
            HttpContext.Current.Response.Write(o.ToXml("xml"));
            HttpContext.Current.Response.End();
        }


        // 订单业务处理
        private static string ProcessOrder(string orderNo)
        {
            Order order = Order.GetDetail(serialNo: orderNo);
            if (order != null && order.Status != (int)OrderStatus.UserPay)
            {
                // 更改订单状态“支付完成”
                order.Pay(OrderPayMode.WechatPay, order.PayMoney, "");

                // 续保业务处理
                if (order.Type == ProductType.Insurance)
                {
                    var item = order?.Items[0];
                    var spec = item.ProductSpec;
                    var asset = item.GetAssets()[0];
                    asset?.AddInsurance(spec);
                    order.Finish();
                }

                // 日志
                var msg = string.Format("微信支付订单处理成功: OrderID={0}, SerialNo={1}", order.ID, order.SerialNo);
                Logger.LogDb("WechatPay", msg, order?.User?.NickName, LogLevel.Info);
                return "";
            }
            else
            {
                var msg = string.Format("微信支付订单处理异常: OrderID={0}, SerialNo={1}", order.ID, order.SerialNo);
                Logger.LogDb("WechatPay", msg, order?.User?.NickName, LogLevel.Warn);
                return "订单处理异常";
            }
        }


        /// <summary>尝试获取并记录用户的UnionId</summary>
        /// <param name="wechatMPId">小程序OpenId</param>
        public static void FetchUserUnionId(string wechatMPId)
        {
            try
            {
                var user = User.Get(wechatMPId: wechatMPId);
                if (user.WechatUnionID.IsNotEmpty())
                    return;

                // 支付成功后5分钟内可获取UnionId
                var unionId = WechatMP.GetPaidUnionId(wechatMPId);
                if (unionId.IsNotEmpty())
                {
                    user.WechatUnionID = unionId;
                    user.Save();
                }
                Logger.LogDb("WechatPay", "GetPaidUnionId：" + unionId, wechatMPId, LogLevel.Info);
            }
            catch
            {
                Logger.LogDb("WechatPay", "GetPaindUnionId fail：", wechatMPId, LogLevel.Info);
            }
        }
    }
}