using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Monitor
{
    public class Win32
    {
        // 是否已经存在进程实例
        public static bool AlreadyExist()
        {
            Process currentProcess = Process.GetCurrentProcess();
            string currentFileName = currentProcess.MainModule.FileName;
            Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (Process process in processes)
                if (process.MainModule.FileName == currentFileName)
                    if (process.Id != currentProcess.Id)
                        return true;
            return false;
        }

    }
}
