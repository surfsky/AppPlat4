using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace App.Components
{
    /// <summary>
    /// WMI 信息类别
    /// </summary>
    internal enum WmiType
    {
        Win32_Processor,
        Win32_PerfFormattedData_PerfOS_Memory,
        Win32_PhysicalMemory,
        Win32_NetworkAdapterConfiguration,
        Win32_LogicalDisk
    }

    /// <summary>
    /// Windows Manager Information
    /// </summary>
    public class WMI
    {
        // wmi info dictionary
        Dictionary<string, ManagementObjectCollection> _dict = new Dictionary<string, ManagementObjectCollection>();
        PerformanceCounter _counter;   //CPU计数器 

        /// <summary>初始化/summary>
        public WMI()
        {
            // 初始化CPU计数器 
            _counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _counter.MachineName = ".";
            _counter.NextValue();

            // 获取设备信息
            var names = Enum.GetNames(typeof(WmiType));
            foreach (string name in names)
            {
                _dict.Add(name, new ManagementObjectSearcher("SELECT * FROM " + name).Get());
            }
        }

        //---------------------------------------
        // CPU
        //---------------------------------------
        /// <summary>获取CPU编码</summary>
        public string GetCPUNumber()
        {
            var query = _dict[WmiType.Win32_Processor.ToString()];
            foreach (var obj in query)
                return obj["Processorid"]?.ToString();
            return "";
        }

        /// <summary>获取CPU信息</summary>
        public string GetCPUInfo()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("内核数 {0}, ", Environment.ProcessorCount);
            sb.AppendFormat("占用率 {0:F2}%, ", GetCPURate());
            var query = _dict[WmiType.Win32_Processor.ToString()];
            foreach (var obj in query)
            {
                sb.AppendFormat("厂商 {0}, ",     obj["Manufacturer"]);
                sb.AppendFormat("产品名称 {0}, ", obj["Name"]);
                sb.AppendFormat("最大频率 {0}, ", obj["MaxClockSpeed"]);
                sb.AppendFormat("当前频率 {0}, ", obj["CurrentClockSpeed"]);
            }
            return sb.ToString().TrimEnd(' ', ',');
        }

        /// <summary>CPU占用率（不准，仅供参考） </summary>
        public float GetCPURate()
        {
            return _counter.NextValue()/Environment.ProcessorCount;
        }


        //---------------------------------------
        // 内存
        //---------------------------------------
        /// <summary>获取内存编码</summary>
        public string GetMemoryNumber()
        {
            var query = _dict[WmiType.Win32_PhysicalMemory.ToString()];
            foreach (var obj in query)
                return obj["PartNumber"]?.ToString();
            return "";
        }


        /// <summary>获取内存信息</summary>
        public string GetMemoryInfo()
        {
            var sb = new StringBuilder();
            var query = _dict[WmiType.Win32_PhysicalMemory.ToString()];

            // 遍历物理内存
            int index = 1;
            double capacity = 0;
            foreach (var obj in query)
            {
                //sb.AppendLine("内存" + index + ", 频率" + obj["ConfiguredClockSpeed"]);
                capacity += Convert.ToDouble(obj["Capacity"]) / 1024 / 1024;
                index++;
            }

            // 内存占有率
            query = _dict[WmiType.Win32_PerfFormattedData_PerfOS_Memory.ToString()];
            double available = 0;
            foreach (var obj in query)
            {
                available += Convert.ToDouble(obj.Properties["AvailableMBytes"].Value);
            }
            sb.AppendFormat("总内存 {0} MB, 可用 {1} MB, 占用率 {2:F2}%", capacity, available, (capacity - available) / capacity * 100);

            return sb.ToString();
        }


        //---------------------------------------
        // 硬盘
        //---------------------------------------
        /// <summary>获取硬盘编码</summary>
        public string GetHardDiskNumber()
        {
            var query = _dict[WmiType.Win32_LogicalDisk.ToString()];
            foreach (var obj in query)
                return obj["VolumeSerialNumber"]?.ToString();
            return "";
        }


        /// <summary>获取硬盘信息</summary>
        public string GetHardDiskInfo()
        {
            var drives = DriveInfo.GetDrives();
            var sb = new StringBuilder();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    var total = (double)drive.TotalSize / 1024 / 1024;
                    var free = (double)drive.TotalFreeSpace / 1024 / 1024;
                    sb.AppendFormat("{0}, 格式{1}, 容量{2}MB, 已用{3}%;\r\n",
                        drive.Name,
                        drive.DriveFormat,
                        (long)total,
                        string.Format("{0:F2}", (total - free) / total * 100)
                        );
                }
            }
            return sb.ToString();
        }


        //---------------------------------------
        // 操作系统
        //---------------------------------------
        /// <summary>获取操作系统信息</summary>
        public string GetOSInfo()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("机器名:{0};操作系统:{1};系统文件夹:{2};语言:{3};.NET:{4};当前目录:{5};当前用户:{6};",
                Environment.MachineName,
                Environment.OSVersion,
                Environment.SystemDirectory,
                CultureInfo.InstalledUICulture.EnglishName,
                Environment.Version,
                Environment.CurrentDirectory,
                Environment.UserName);
            return sb.ToString();
        }



        //---------------------------------------
        // 网络
        //---------------------------------------
        /// <summary>获取网卡信息</summary>
        //public static string GetNetworkInfo()
        //{
        //    StringBuilder sr = new StringBuilder();

        //    string host = Dns.GetHostName();
        //    IPHostEntry ipEntry = Dns.GetHostByName(host);
        //    sr.Append("IPv4:" + ipEntry.AddressList[0] + "/");

        //    sr.Append("IPv6:");
        //    ipEntry = Dns.GetHostEntry(host);
        //    sr.Append("IPv6:" + ipEntry.AddressList[0] + ";");

        //    sr.Append("MAC:");
        //    var query = WmiDict[WmiType.Win32_NetworkAdapterConfiguration.ToString()];
        //    foreach (var obj in query)
        //    {
        //        if (obj["IPEnabled"].ToString() == "True")
        //            sr.Append(obj["MacAddress"] + ";");
        //    }
        //    return sr.ToString();
        //}
    }
}