using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;
using App.DAL;
using App.Scheduler;


namespace App.Jobs
{
    /// <summary>文章定时修复任务</summary>
    public class ArticleJob : IJobRunner
    {
        /// <summary>定时调度接口</summary>
        public bool Run(DateTime dt, string data)
        {
            var n = new Article().Fix();
            Console.WriteLine("修正文章数据：" + n.ToString());
            return true;
        }
    }
}
