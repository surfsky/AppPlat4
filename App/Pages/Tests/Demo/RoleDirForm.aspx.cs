using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Controls;
using App.Components;
using App.Entities;

namespace App.Tests
{
    [UI("角色可访问的目录表单（UISetting Form 示例）")]
    [Auth(Powers.ArticleDirRoleEdit)]
    public partial class RoleDirForm : FormPage<ArticleDirRole>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var ui = new UISetting<ArticleDirRole>(true);
            ui.SetEditorWinGrid(t => t.RoleID, typeof(Role), nameof(Role.Name));
            ui.SetEditorWin(t => t.ArticleDirID, typeof(ArticleDir), nameof(ArticleDir.FullName), "/Pages/Articles/ArticleDirs.aspx", "选择目录");
            this.form2.Build(ui);
        }
    }
}
