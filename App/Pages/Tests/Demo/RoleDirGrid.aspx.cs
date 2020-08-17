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
    [UI("角色可访问目录（UISetting Grid示例）")]
    [Auth(Powers.ArticleDirRoleEdit)]
    public partial class RoleDirGrid : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            var ui = new UISetting<ArticleDirRole>(false);
            ui.SetColumn(t => t.Role.Name);
            ui.SetColumn(t => t.ArticleDir.FullName);
            this.grid1.SetUI(ui).SetPowers(this.Auth).SetUrls("RoleDirForm.aspx").Build();
            if (!IsPostBack)
            {
                grid1.SetSortPage(SiteConfig.Instance.PageSize);
                grid1.BindGrid();
            }
        }
    }
}
