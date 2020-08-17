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

namespace App.Open
{
    [UI("开放平台-App管理")]
    [Auth(Powers.Admin)]
    public partial class Apps : MixPage<OpenApp>
    {
        protected override UISetting GetGridUI()
        {
            var ui = new UISetting<OpenApp>(true);
            //ui.SetColumn(t => t.Role.Name);
            //ui.SetColumn(t => t.ArticleDir.FullName);
            return ui;
        }

        protected override UISetting GetFormUI()
        {
            var ui = new UISetting<OpenApp>(true);
            //ui.SetEditorWinGrid(t => t.RoleID, typeof(Role), nameof(Role.Name));
            //ui.SetEditorWin(t => t.ArticleDirID, typeof(ArticleDir), nameof(ArticleDir.FullName), "/Pages/Articles/ArticleDirs.aspx");
            return ui;
        }
    }
}
