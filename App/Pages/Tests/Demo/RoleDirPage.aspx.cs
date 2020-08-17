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

namespace App.Tests
{
    [UI("角色可访问目录（MixPage 原型）")]
    [Auth(Powers.ArticleDirRoleEdit)]
    public partial class RoleDirPage : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            Switch();
        }

        private void Switch()
        {
            var v = Asp.GetQueryString("v");
            if (v == "Form")
            {
                grid.Hidden = true;
                PageManager.Instance.AutoSizePanelID = nameof(form);
                ShowForm();
            }
            else
            {
                form.Hidden = true;
                PageManager.Instance.AutoSizePanelID = nameof(grid);
                ShowGrid();
            }
        }

        private void ShowGrid()
        {
            var ui = new UISetting<ArticleDirRole>(false);
            ui.SetColumn(t => t.Role.Name);
            ui.SetColumn(t => t.ArticleDir.FullName);
            this.grid.SetUI(ui).SetPowers(this.Auth).SetUrls("RoleDirPage.aspx?v=Form").Build();
            if (!IsPostBack)
            {
                grid.SetSortPage(SiteConfig.Instance.PageSize);
                grid.BindGrid();
            }
        }

        private void ShowForm()
        {
            var ui = new UISetting<ArticleDirRole>(true);
            ui.SetEditorWinGrid(t => t.RoleID, typeof(Role), nameof(Role.Name));
            ui.SetEditorWin(t => t.ArticleDirID, typeof(ArticleDir), nameof(ArticleDir.FullName), "/Pages/Articles/ArticleDirs.aspx");
            this.form.Build(ui);
        }
    }
}
