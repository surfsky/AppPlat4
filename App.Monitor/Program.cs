using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using App.Scheduler;
using App.Utils;
using App.DAL;
using App.Entities;

namespace App.Monitor
{
    class Program
    {
        // 主入口
        static void Main(string[] args)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("App.Monitor {0}", version);
            Console.WriteLine("Engine version {0}", App.Scheduler.ScheduleEngine.Version);
            Console.WriteLine("Last update : 2020-03-19");

            // 单例检测
            if (Win32.AlreadyExist())
            {
                Console.WriteLine("Already exist.");
                return;
            }

            // 配置和启动
            //var db = new AppContext("db");
            UtilConfig.Instance.MachineId = IO.GetAppSetting<int>("MachineID");
            EntityConfig.Instance.OnGetDb += () => AppContext.Current;
            App.Components.Logger.LogDb("Monitor-Start");
            var engine = CreateEngine();
            engine.JobRunning += (job, info) => Logger.Info("[{0}] {1} running", job.Name, job.Data);
            engine.JobSuccess += (job, info) => Logger.Info("[{0}] {1} ok", job.Name, job.Data);
            engine.JobFailure += (job, info) => Logger.Warn("[{0}] {1} fail, times={2}, info={3}", job.Name, job.Data, job.Failure.TryTimes, info);
            engine.Start();
        }


        /// <summary>用代码构建引擎配置</summary>
        static ScheduleEngine CreateEngine()
        {
            ScheduleConfig cfg = new ScheduleConfig();
            cfg.LogDt = DateTime.Now;
            cfg.Sleep = 200;
            cfg.Jobs = new List<Job>();
            cfg.Jobs.Add(new Job()
            {
                Name = "ConnectJob",
                Runner = typeof(ConnectJob),
                Success = new DateSpan(0, 0, 0, 0, 10, 0),
                Failure = new DateSpan(0, 0, 0, 0, 10, 0, 0, 1),
                Schedule = new Schedule("* * * * * *"),
                Data = "http://doc.oa.wz.zj.cn:88",
            });
            cfg.Jobs.Add(new Job()
            {
                Name = "ArticleJob",
                Runner = typeof(App.Jobs.ArticleJob),
                Success = new DateSpan(0, 0, 0, 0, 10, 0),
                Failure = new DateSpan(0, 0, 0, 0, 0, 0, 0, 9),
                Schedule = new Schedule("* * * * * *")
            });
            cfg.Jobs.Add(new Job()
            {
                Name = "LogJob",
                Runner = typeof(App.Jobs.LogJob),
                Success = new DateSpan(0, 0, 10, 0, 0, 0),
                Failure = new DateSpan(0, 0, 0, 0, 0, 0, 0, 9),
                Schedule = new Schedule("* * * * * *")
            });
            return new ScheduleEngine(cfg);
        }
    }
}
