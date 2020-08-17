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
// 订单任务
//---------------------------------------------------------
namespace App.Jobs
{
    public class OrderJob : IJobRunner
    {
        public bool Run(DateTime dt, string data)
        {
            //AppContext.Release();
            var startDt = DateTime.Now.AddDays(-30);
            var orders = Order.Search((int)OrderStatus.Create, startDt: startDt).Sort(t => t.CreateDt, false).ToList();
            foreach (var o in orders)
                o.FixItem();
            return true;
        }
    }

}
