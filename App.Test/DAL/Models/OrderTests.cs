using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;
using App.Entities;

namespace App.DAL.Tests
{
    [TestClass()]
    public class OrderTests
    {
        [TestMethod()]
        public void BuildSerialNoTest()
        {
            IO.Trace(new Order().ToJson());
            IO.Trace(Order.BuildSerialNo());
        }
    }
}