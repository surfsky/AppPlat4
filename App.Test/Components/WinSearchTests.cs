using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;

namespace App.Components.Tests
{
    [TestClass()]
    public class WinSearchTests
    {
        [TestMethod()]
        public void SearchTest()
        {
            var data = WinSearch.Search(@"C:\Files", "test1 test2".SplitString());
            IO.Write(data.ToJson());
        }
    }
}