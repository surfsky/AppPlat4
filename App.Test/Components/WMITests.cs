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
    public class WMITests
    {
        [TestMethod()]
        public void WMITest()
        {
            var wmi = new WMI();
            IO.Write("OS: {0}", wmi.GetOSInfo());
            IO.Write("Memory: {0}", wmi.GetMemoryInfo());
            IO.Write("Disk: {0}", wmi.GetHardDiskInfo());
            IO.Write("CPUNumber: {0}", wmi.GetCPUNumber());
            IO.Write("DiskNumber: {0}", wmi.GetHardDiskNumber());
            IO.Write("MemoryNumber: {0}", wmi.GetMemoryNumber());
        }
        
    }
}