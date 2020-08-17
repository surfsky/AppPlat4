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
    public class SystemInfoTests
    {
        //[TestMethod()]
        public void SystemInfoTest()
        {
            var info = new SystemInfo();
            IO.Write("CPURate: {0}", info.CPURate);
            IO.Write("CPUProcessors: {0}", info.CPUProcessors);
            IO.Write("MemoryRate: {0}", info.MemoryRate);
            IO.Write("Drivers: {0}", info.GetDrives().ToJson());
            IO.Write("Processes: {0}", info.GetProcesses().ToJson());
        }


    }
}