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
using App.Components;
using App.Controls;

namespace App.Admins
{
    [UI("阿里短信接入配置")]
    [Auth(Powers.Admin)]
    public partial class SmsConfigForm : PageBase
    {
        // 初始化
        protected void Page_Load(object sender, EventArgs e)
        {
            var ui = new UISetting<AliSmsConfig>(false);
            ui.SetEditor(t => t.SmsAccessKeyId);
            ui.SetEditor(t => t.SmsAccessKeySecret);
            ui.SetEditor(t => t.SmsSignName);
            ui.SetEditor(t => t.SmsChangeInfo);
            ui.SetEditor(t => t.SmsChangePassword);
            ui.SetEditor(t => t.SmsNotify);
            ui.SetEditor(t => t.SmsRegist);
            ui.SetEditor(t => t.SmsVerify);
            this.form2.ShowIdLabel = false;
            this.form2.ShowCloseButton = false;
            this.form2.Mode = PageMode.Edit;
            this.form2.Build(AliSmsConfig.Instance, ui);

        }
    }
}
