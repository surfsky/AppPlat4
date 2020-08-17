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
using App.Components;
using App.Controls;
using App.Utils;

namespace App.Admins
{
    [UI("角色可访问目录")]
    [Auth(Powers.ArticleDirRoleEdit)]
    public partial class RoleDirs : MixPage<ArticleDirRole>
    {
        protected override UISetting GetGridUI()
        {
            var ui = new UISetting<ArticleDirRole>(false);
            ui.SetColumn(t => t.Role.Name);
            ui.SetColumn(t => t.ArticleDir.FullName);
            return ui;
        }

        protected override UISetting GetFormUI()
        {
            var ui = new UISetting<ArticleDirRole>(true);
            ui.SetEditorWinGrid(t => t.RoleID, typeof(Role), nameof(Role.Name));
            ui.SetEditorWin(t => t.ArticleDirID, typeof(ArticleDir), nameof(ArticleDir.FullName), "/Pages/Articles/ArticleDirs.aspx");
            return ui;
        }
    }
}
