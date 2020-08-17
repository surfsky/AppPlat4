using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Components.Tests
{
    [TestClass()]
    public class QrCodeTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            var code = new QrCodeData(QrCodeType.Invite, "userId=1", "邀请", null);
            var text = code.ToString();
            code = QrCodeData.Parse(text);
        }
    }
}