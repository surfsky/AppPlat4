using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.UI;
using FineUIPro;

namespace App.Controls
{
    /// <summary>
    /// 日期时间选择框。用法同DatePicker。
    /// FineUIPro 企业版
    ///     http://pro.fineui.com/#/form/datepicker_time_showsecond.aspx
    ///     DatePicker 有个属性 ShowTime=True 就可以显示时间了
    /// 免费版只能集成类似 My97 的控件：
    ///     http://pro.fineui.com/#/third-party/my97/my97.aspx
    /// </summary>
    public class DateTimePicker : TriggerBox
    {
        public DateTime? SelectedDate
        {
            get
            {
                try { return Convert.ToDateTime(this.Text); }
                catch { return null; }
            }
            set { this.Text = value?.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        protected override void OnInit(EventArgs e)
        {
            this.TriggerIcon = TriggerIcon.Date;
            this.EnableEdit = false;
            base.OnInit(e);

            // script tag
            var js = "/res/third-party/my97/WdatePicker.js";
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("WDatePicker"))
                Page.ClientScript.RegisterClientScriptInclude("WDatePicker", js);

            // client event
            var script = string.Format(@"
                var picker = F('{0}');
                WdatePicker({{
                    el: '{0}-inputEl',
                    dateFmt: 'yyyy-MM-dd HH:mm:ss',
                    onpicked: function() {{
                        picker.validate();
                    }}
                }});
                ", this.ClientID)
                ;
            this.OnClientTriggerClick = script;
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), ClientID, script, true);  // 用这种方式输出，位置靠前, 控件未创建，会报错
        }
    }
}