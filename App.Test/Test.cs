using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using App.Components;
using App.Utils;
using App.DAL;
using System.Collections.Generic;
using System.Net;

namespace App.Test
{
    /// <summary>
    /// 基础类库测试
    /// </summary>
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestGithub()
        {
            var markdown = @"
# header1

- list item
- list item
- list item
";
            var html = RenderGithubMarkdow(markdown);
        }

        static public String RenderGithubMarkdow(String markdown)
        {
            var text = new { text = markdown, mode = "markdown" }.ToJson();
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "ghmd-renderer");
            webClient.Headers.Add("Content-Type", "text/x-markdown");
            return webClient.UploadString("https://api.github.com/markdown", "POST", text);  // System.Net.WebException:“请求被中止: 未能创建 SSL/TLS 安全通道。”
        }



        [TestMethod]
        public void TestCast()
        {
            var now = DateTime.Now;
            var dts = new List<DateTime>() { now, now.AddDays(1), now.AddDays(2) };
            var ints = new List<int>() { 1, 2, 3, 4, 5, 6 };
            var strings = new List<string>() { "1", "2", "3", "4", "5", "6" };
            var enums = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday };

            var o1 = dts.CastString();
            var o2 = enums.CastInt();
            var o3 = enums.CastString();
            var o4 = strings.CastInt();
            var o5 = ints.CastEnum<DayOfWeek>();
            var o6 = strings.CastEnum<DayOfWeek>();
            var o7 = strings.Cast<string, DayOfWeek>(t => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), t));
        }

        [TestMethod]
        public void TestDictionary()
        {
            var dict = new Dictionary<string, string>();
            dict["Name"] = "Kevin";
            dict["Sex"] = "Male";
            IO.Write(dict["Name"]);
            IO.Write(dict["Birthday"]);
        }
    }
}
