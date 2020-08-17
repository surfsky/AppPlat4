using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Components;

namespace App.Admins
{
    [UI("黑名单")]
    [Auth(Powers.IPFilter)]
    public partial class IPFilterForm : FormPage<IPFilter>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
                ShowForm();
        }

        public override void NewData()
        {
            var ip = Asp.GetQueryString("ip");
            var dt = Asp.GetQueryString("dt");
            var logId = Asp.GetQueryString("logId");
            var remark = string.Format("检测到攻击. dt={0}，logId={1}", dt, logId);

            UI.SetValue(this.tbIP, ip);
            UI.SetValue(this.dtStart, DateTime.Now);
            UI.SetValue(this.dtEnd, null);
            UI.SetValue(this.tbAddr, "");
            UI.SetValue(this.tbRemark, remark);
        }

        public override void ShowData(IPFilter item)
        {
            UI.SetValue(this.tbIP, item.IP);
            UI.SetValue(this.dtStart, item.StartDt);
            UI.SetValue(this.dtEnd, item.EndDt);
            UI.SetValue(this.tbRemark, item.Addr);
            UI.SetValue(this.tbRemark, item.Remark);
        }

        public override void CollectData(ref IPFilter item)
        {
            item.IP = UI.GetText(tbIP);
            item.StartDt = UI.GetDate(dtStart);
            item.EndDt = UI.GetDate(dtEnd);
            item.Addr = UI.GetText(tbAddr);
            item.Remark = UI.GetText(tbRemark);
        }

        // IP归属地理位置查询
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var ip = UI.GetText(tbIP);
            if (ip.IsNotEmpty())
                this.tbAddr.Text = IPQuerier.Query(ip);
        }
    }
}
