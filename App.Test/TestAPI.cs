using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using App.Components;
using App.Utils;
using App.DAL;

namespace App.Test
{
    /// <summary>
    /// API 接口测试
    /// </summary>
    [TestClass]
    public class TestAPI
    {
        [TestMethod]
        public void TestThumbnail()
        {
            var img = HttpHelper.GetThumbnail("https://ss0.bdstatic.com/5aV1bjqh_Q23odCf/static/superman/img/logo_top_ca79a146.png", 20, 20);
            img = HttpHelper.GetThumbnail("../Res/images/defaultUser.png", 20, 20);
        }
    }
}
