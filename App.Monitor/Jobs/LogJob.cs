using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using App.Utils;
using App.DAL;
using App.Scheduler;

//---------------------------------------------------------
// 各种定时后台任务。由App.Consoler.exe运行，配置写在Schedule.config中
//---------------------------------------------------------
namespace App.Jobs
{
    /// <summary>清除日志任务。每月1日运行</summary>
    /// <remarks>log4net的日志不用写代码清理，可在配置中设置 MaxSizeRollBackups 参数</remarks>
    public class LogJob : IJobRunner
    {
        public bool Run(DateTime dt, string data)
        {
            //AppContext.Release();
            Log.DeleteBatch(1);
            return true;
        }
    }

}
