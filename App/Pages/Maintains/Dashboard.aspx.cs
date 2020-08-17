using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
//using EntityFramework.Extensions;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;
using System.IO;
using System.Collections;
using System.Text;
using App.HttpApi;

namespace App.Admins
{
    [Auth(Powers.AdminMonitor)]
    [UI("仪表盘页面")]
    public partial class Dashboard : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.lbl1.Text = BuildRuningInfo();
                this.lbl2.Text = BuildAssemblyInfo();
                this.lbl3.Text = Asp.BuildServerInfo("");
                //this.lbl4.Text = Common.BuildBootstrapCss(false);
            }
        }

        /// <summary>显示运行信息</summary>
        public string BuildRuningInfo()
        {
            var data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("Host", Asp.Host));
            data.Add(new KeyValuePair<string, string>("IP", Net.IPs?[0]));
            data.Add(new KeyValuePair<string, string>("MachineId", UtilConfig.Instance.MachineId.ToString()));
            data.Add(new KeyValuePair<string, string>("StartDt", Global.StartDt.ToString("yyyy-MM-dd HH:mm:ss")));
            data.Add(new KeyValuePair<string, string>("Duration", (DateTime.Now-Global.StartDt).ToString()));
            data.Add(new KeyValuePair<string, string>("Internet", Net.Ping("8.8.8.8").ToString()));
            data.Add(new KeyValuePair<string, string>("DNS", Net.Ping("www.baidu.com").ToString()));
            data.Add(new KeyValuePair<string, string>("Location", Server.MapPath("~/")));
            return WebHelper.BuildBootstrapTable(data);
        }

        // 显示系统关键程序集信息
        private string BuildAssemblyInfo()
        {
            var assembly1 = typeof(App.Global).Assembly.GetName();
            var assembly2 = typeof(App.Utils.Convertor).Assembly.GetName();
            var assembly3 = typeof(App.Entities.EntityBase).Assembly.GetName();
            var assembly4 = typeof(App.DAL.AppContext).Assembly.GetName();
            var assembly5 = typeof(App.HttpApi.HttpApiHelper).Assembly.GetName();
            var assembly6 = typeof(App.Wechats.Wechat).Assembly.GetName();

            var data = new List<object>
            {
                new {Name=assembly1.Name, Version=assembly1.Version.ToString()},
                new {Name=assembly2.Name, Version=assembly2.Version.ToString()},
                new {Name=assembly3.Name, Version=assembly3.Version.ToString()},
                new {Name=assembly4.Name, Version=assembly4.Version.ToString()},
                new {Name=assembly5.Name, Version=assembly5.Version.ToString()},
                new {Name=assembly6.Name, Version=assembly6.Version.ToString()},
            };
            return WebHelper.BuildBootstrapTable(data);
        }

        [HttpApi("系统状态")]
        public static string GetComputerInfo()
        {
            var wmi = new WMI();
            var info = string.Format("<h5>CPU</h5>{0}<h5>内存</h5>{1}<h5>硬盘</h5>{2}",
                wmi.GetCPUInfo(),
                wmi.GetMemoryInfo(),
                wmi.GetHardDiskInfo()
                );
            return info;
        }

        // 重启网站
        protected void btnReboot_Click(object sender, EventArgs e)
        {
            Asp.RebootSite();
        }


        // 清除缓存
        protected void btnClearCache_Click(object sender, EventArgs e)
        {
            var n = FileCacher.ClearFileCaches(true);
            UI.ShowAlert($"成功删除缓存文件{n}个");
        }


    }
}
