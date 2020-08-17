using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App;
using System.Reflection;
using System.Collections;
using App.Utils;
using App.Controls;
using App.Components;


namespace App.Admins
{
    /// <summary>
    /// 日志查看窗口（只读）
    /// LogForm.aspx?id=xxxx&md=view
    /// </summary>
    [UI("日志")]
    [Auth(Powers.Log)]
    public partial class LogForm : FormPage<Log>
    {
        // 初始化
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Mode = PageMode.View;
            InitForm(this.form2, this.form2, Powers.Log, Powers.Log, false, false);
            if (!IsPostBack)
                ShowForm();
        }

        // 加载数据
        public override void ShowData(Log item)
        {
            UI.SetValue(lblId, item.ID);
            UI.SetValue(tbLevel, item.Level);
            UI.SetValue(tbOperator, item.Operator);
            UI.SetValue(tbSummary, item.Summary);
            UI.SetValue(tbURL, item.URL);
            UI.SetValue(tbMethod, item.Method);
            UI.SetValue(tbReferrer, item.Referrer);
            UI.SetValue(tbIP, item.IP);
            UI.SetValue(tbFrom, item.From);
            UI.SetValue(tbLogDt, item.LogDt);
            UI.SetValue(tbMsg, item.Message);
            UI.SetValue(tbMsgHtml, item.Message);
        }

        // 加入黑名单
        protected void btnBan_Click(object sender, EventArgs e)
        {
            var ip = UI.GetText(tbIP);
            var dt = UI.GetText(tbLogDt);
            var id = Asp.GetQueryLong("ID");
            var url = string.Format("IPFilterForm.aspx?md=new&ip={0}&dt={1}&logId={2}", ip, dt, id);
            UI.ShowWindow(this.win, url, "加入黑名单");
        }

        // 切换文本和Html显示方式
        protected void btnSwitch_Click(object sender, EventArgs e)
        {
            bool willShowHtml = tbMsgHtml.Hidden;
            tbMsg.Hidden = willShowHtml;
            tbMsgHtml.Hidden = !willShowHtml;
        }
    }
}
