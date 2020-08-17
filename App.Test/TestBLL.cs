using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using App.Components;
using App.Utils;
using App.DAL;

namespace App.Test
{
    /// <summary>
    /// 业务逻辑测试
    /// </summary>
    [TestClass]
    public class TestBLL
    {
        // 测试阿里云短信接口（用代码发送只收到第一条短信，单步调试没问题）
        //[TestMethod]
        public void TestSms()
        {
            AliSmsMessenger.SendSmsRegist("15305770121", "12345");
            System.Threading.Thread.Sleep(3000);
            AliSmsMessenger.SendSmsVerify("15305770121", "12345");
            System.Threading.Thread.Sleep(3000);
            AliSmsMessenger.SendSmsChangePassword("15305770121", "12345");
            System.Threading.Thread.Sleep(3000);
            AliSmsMessenger.SendSmsChangeInfo("15305770121", "12345");
        }
    }
}
