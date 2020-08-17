using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;

namespace App.Components
{
    /// <summary>
    /// 磁盘信息
    /// </summary>
    public class DiskInfo
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public long FreeSpace { get; set; }

        public DiskInfo(string name, long size, long freeSpace)
        {
            this.Name = name;
            this.Size = size;
            this.FreeSpace = FreeSpace;
        }
    }

    /// <summary>
    /// 进程信息
    /// </summary>
    public class ProcessInfo
    {
        public long Id { get; set; }
        public string ProcessName { get; set; }
        public double TotalMilliseconds { get; set; }
        public long WorkingSet64 { get; set; }
        public string FileName { get; set; }

        public ProcessInfo(long id, string processName, double totalMillisenconds, long workingset64, string fileName)
        {
            this.Id = id;
            this.ProcessName = processName;
            this.TotalMilliseconds = totalMillisenconds;
            this.WorkingSet64 = workingset64;
            this.FileName = fileName;
        }
    }


    /// <summary>
    /// 系统信息类 - 获取CPU、内存、磁盘、进程信息
    /// </summary>
    public class SystemInfo
    {
        private int _processorCount = 0;   //CPU个数 
        private PerformanceCounter _counter;   //CPU计数器 
        private long _physicalMemory = 0;   //物理内存 
        private const int GW_HWNDFIRST = 0;
        private const int GW_HWNDNEXT = 2;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 268435456;
        private const int WS_BORDER = 8388608;

        #region API声明 
        [DllImport("IpHlpApi.dll")]
        extern static public uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

        [DllImport("User32")]
        private extern static int GetWindow(int hWnd, int wCmd);

        [DllImport("User32")]
        private extern static int GetWindowLongA(int hWnd, int wIndx);

        [DllImport("user32.dll")]
        private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static int GetWindowTextLength(IntPtr hWnd);
        #endregion

        /// 构造函数，初始化计数器等 
        public SystemInfo()
        {
            // 初始化CPU计数器 
            _counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _counter.MachineName = ".";
            _counter.NextValue();

            // CPU个数 
            _processorCount = Environment.ProcessorCount;

            // 获得物理内存 
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                    _physicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());
            }
        }

        //-----------------------------------------
        // CPU
        //-----------------------------------------
        /// <summary>CPU个数 </summary>
        public int CPUProcessors
        {
            get {return _processorCount;}
        }

        /// <summary>CPU占用率 </summary>
        public float CPURate
        {
            get {return _counter.NextValue();}
        }


        //-----------------------------------------
        // 内存
        //-----------------------------------------
        /// <summary>内存占用率</summary>
        public float MemoryRate
        {
            get { return (float)((_physicalMemory - AvailableMemory) * 1.0 / _physicalMemory); }
        }

        /// <summary>物理内存</summary>
        public long PhysicalMemory
        {
            get { return _physicalMemory; }
        }

        /// <summary>可用内存 </summary>
        public long AvailableMemory
        {
            get
            {
                long availablebytes = 0;
                ManagementClass mos = new ManagementClass("Win32_OperatingSystem");
                foreach (ManagementObject mo in mos.GetInstances())
                {
                    if (mo["FreePhysicalMemory"] != null)
                        availablebytes = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());
                }
                return availablebytes;
            }
        }


        //-----------------------------------------
        // 磁盘
        //-----------------------------------------
        /// <summary>获取分区信息 </summary>
        public List<DiskInfo> GetDrives()
        {
            var drives = new List<DiskInfo>();
            ManagementClass diskClass = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection disks = diskClass.GetInstances();
            foreach (ManagementObject disk in disks)
            {
                // DriveType.Fixed 为固定磁盘(硬盘) 
                if (int.Parse(disk["DriveType"].ToString()) == (int)DriveType.Fixed)
                {
                    drives.Add(new DiskInfo(
                        disk["Name"].ToString(), 
                        long.Parse(disk["Size"].ToString()), 
                        long.Parse(disk["FreeSpace"].ToString())
                        ));
                }
            }
            return drives;
        }

        /// <summary>获取特定分区信息</summary>
        public List<DiskInfo> GetDrives(char DriverID)
        {
            var drives = new List<DiskInfo>();
            WqlObjectQuery query = new WqlObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DeviceID = ’" + DriverID + ":’");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject disk in searcher.Get())
            {
                if (int.Parse(disk["DriveType"].ToString()) == (int)DriveType.Fixed)
                {
                    drives.Add(new DiskInfo(
                        disk["Name"].ToString(), 
                        long.Parse(disk["Size"].ToString()), 
                        long.Parse(disk["FreeSpace"].ToString())
                        ));
                }
            }
            return drives;
        }


        //-----------------------------------------
        // 进程相关
        //-----------------------------------------
        /// <summary>获得进程列表 </summary>
        public List<ProcessInfo> GetProcesses()
        {
            var pInfo = new List<ProcessInfo>();
            Process[] processes = Process.GetProcesses();
            foreach (Process instance in processes)
            {
                try
                {
                    pInfo.Add(new ProcessInfo(
                        instance.Id,
                        instance.ProcessName,
                        instance.TotalProcessorTime.TotalMilliseconds,
                        instance.WorkingSet64,
                        instance.MainModule.FileName));
                }
                catch { }
            }
            return pInfo;
        }

        /// <summary>获得特定进程信息</summary>
        public List<ProcessInfo> GetProcesses(string ProcessName)
        {
            var pInfo = new List<ProcessInfo>();
            Process[] processes = Process.GetProcessesByName(ProcessName);
            foreach (Process instance in processes)
            {
                try
                {
                    pInfo.Add(new ProcessInfo(
                        instance.Id,
                        instance.ProcessName,
                        instance.TotalProcessorTime.TotalMilliseconds,
                        instance.WorkingSet64,
                        instance.MainModule.FileName));
                }
                catch { }
            }
            return pInfo;
        }

        /// <summary>查找所有应用程序标题</summary>
        public static List<string> FindAllApps(int Handle)
        {
            var apps = new List<string>();
            int hwCurr;
            hwCurr = GetWindow(Handle, GW_HWNDFIRST);

            while (hwCurr > 0)
            {
                int IsTask = (WS_VISIBLE | WS_BORDER);
                int lngStyle = GetWindowLongA(hwCurr, GWL_STYLE);
                bool TaskWindow = ((lngStyle & IsTask) == IsTask);
                if (TaskWindow)
                {
                    int length = GetWindowTextLength(new IntPtr(hwCurr));
                    var sb = new StringBuilder(2 * length + 1);
                    GetWindowText(hwCurr, sb, sb.Capacity);
                    string strTitle = sb.ToString();
                    if (!string.IsNullOrEmpty(strTitle))
                        apps.Add(strTitle);
                }
                hwCurr = GetWindow(hwCurr, GW_HWNDNEXT);
            }

            return apps;
        }



        /// <summary>结束指定进程</summary>
        public static void EndProcess(int pid)
        {
            try
            {
                Process process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch { }
        }

    }
}