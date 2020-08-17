using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Wechats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;
using App.Wechats.OP;

namespace App.Wechats.Tests
{
    [TestClass()]
    public class WechatWebTests
    {
        [TestMethod()]
        public void TryGetTagTest()
        {
            var tagName = "测试标签";
            var openId = "oaL9vxFPU4pHMMvIiYMMz7cropgw";
            WechatOP.TrySetUserTag(openId, tagName);
        }


        [TestMethod()]
        public void ParseWechatXmlTest()
        {
            var xml = @"<xml><ToUserName><![CDATA[gh_ae1207a05405]]></ToUserName>
                <FromUserName><![CDATA[oaL9vxImyL4JKm6Xobz-rYx4XVIE]]></FromUserName>
                <CreateTime>1555166378</CreateTime>
                <MsgType><![CDATA[text]]></MsgType>
                <Content><![CDATA[手机]]></Content>
                <MsgId>22264625617127345</MsgId>
                </xml>";
            var o = xml.ParseXml<PushMessage>();
            IO.Write(o.ToJson());
        }

    }
}