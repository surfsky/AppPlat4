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
    /// <summary>
    /// 微信配置窗口
    /// </summary>
    [Auth(Powers.Admin)]
    [UI("")]
    public partial class DingConfigForm : PageBase
    {
        // 初始化
        protected void Page_Load(object sender, EventArgs e)
        {
            this.form2.ShowIdLabel = false;
            this.form2.Mode = PageMode.Edit;
            this.form2.Build(AliDingConfig.Instance);
        }
    }
}
