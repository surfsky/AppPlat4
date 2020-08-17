using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Wechats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;
using App.Wechats.Pay;

namespace App.Wechats.Tests
{
    [TestClass()]
    public class WechatPayTests
    {
        string _mchKey = "xiaoxiong12345678910111213141516";

        [TestMethod()]
        public void UnifiedOrderReplyTest()
        {
            var xml = @"
                <xml>
                  <appid>wx0253f291b14e43dc</appid>
                  <body>续保服务 等1件商品</body>
                  <mch_id>1488073052</mch_id>
                  <nonce_str>2bef677c2dc249cba2956e6c908d0019</nonce_str>
                  <notify_url>https://www.bearmanager.cn/WeiXin/Pay.ashx</notify_url>
                  <openid>oVSMv5aydM4Mz9Geuf8BzHg5-kQg</openid>
                  <out_trade_no>20190412000060</out_trade_no>
                  <spbill_create_ip>60.180.190.97</spbill_create_ip>
                  <total_fee>7900</total_fee>
                  <trade_type>JSAPI</trade_type>
                  <sign>DCBFCCA7F4095B86C4558ED7D07D7AD9</sign>
                </xml>
                ";
            var reply = xml.ParseXml<UnifiedOrderReply>();   // 请求类型未定义
            var b = WechatPay.CheckPaySign(xml, _mchKey);
        }

        [TestMethod()]
        public void PayCallbackTest()
        {
            var xml = @"
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
                ";
            var o = xml.ParseXml<PayCallback>();
            var b = WechatPay.CheckPaySign(xml, _mchKey);
        }


        [TestMethod()]
        public void BuildSignTest()
        {
            var mchKey = "mchKey";
            var nonceStr = WechatPay.BuildNonceStr();
            var tradeType = "JSAPI";
            var fee = 120.3d;
            var dict = new Dictionary<string, string>();
            dict.Add("appid", "appid");
            dict.Add("body", "body");
            dict.Add("mch_id", "mch_id");
            dict.Add("nonce_str", nonceStr);
            dict.Add("notify_url", "notify_url");
            dict.Add("openid", "openid");
            dict.Add("out_trade_no", "out_trade_no");
            dict.Add("spbill_create_ip", "spbill_create_ip");
            dict.Add("total_fee", Convert.ToInt32(fee * 100).ToString());
            dict.Add("trade_type", tradeType);
            var sign = WechatPay.BuildPaySign(dict, mchKey);
            dict.Add("sign", sign);
            string xml = dict.ToXml("xml");
            bool b = WechatPay.CheckPaySign(xml, mchKey);
            System.Diagnostics.Trace.WriteLine(xml);
        }
    }
}