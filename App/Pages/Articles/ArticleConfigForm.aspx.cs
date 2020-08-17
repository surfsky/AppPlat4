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
    [Auth(Powers.ArticleEdit)]
    [UI("文档库配置")]
    public partial class ArticleConfigForm : PageBase
    {
        // 初始化
        protected void Page_Load(object sender, EventArgs e)
        {
            this.form2.ShowIdLabel = false;
            this.form2.Mode = PageMode.Edit;
            this.form2.Build(ArticleConfig.Instance);
        }
    }
}
