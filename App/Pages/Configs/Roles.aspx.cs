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
    [UI("角色管理")]
    [Auth(Powers.RolePowerEdit)]
    public partial class Roles : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            var ui = new UISetting<Role>(true);
            this.Grid1.SetUI(ui).SetPowers(this.Auth).SetAutoUrls().Build();
            if (!IsPostBack)
            {
                Grid1.SetSortPage(SiteConfig.Instance.PageSize);
                Grid1.BindGrid();
            }
        }
    }
}
