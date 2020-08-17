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
using App.Controls;
using App.Utils;
using App.Components;
using System.Reflection;

namespace App.Pages
{
    [UI("组织管理 UISetting 方案演示")]
    [Auth(Powers.OrgView, Powers.OrgNew, Powers.OrgEdit, Powers.OrgDelete)]
    public partial class Orgx : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // grid ui
            var ui = new UISetting<Org>(false);
            ui.SetColumn(t => t.Name, 150, sort: true);
            ui.SetColumn(t => t.City, 150);
            ui.SetColumn(t => t.Addr, 200);
            ui.SetColumn(t => t.CertNo, 80);
            ui.SetColumnImage(t => t.CertPic);
            ui.SetColumn(t => t.LegalPerson, 80);
            ui.SetColumn(t => t.LegalPersonIDCardNo, 80);
            ui.SetColumnImage(t => t.LegalPersonIDCardPic);
            ui.SetColumn(t => t.LegalPersonTel, 80);
            ui.SetColumn(t => t.Approved, 80);

            // search ui
            var m = typeof(Org).GetMethod("Search");
            var searchUI = new UISetting(m);

            // init
            Grid1.SetUI(ui)
                .SetSearcher(searchUI)
                .SetPowers(this.Auth)
                .SetUrls("OrgForm.aspx")
                .Build();

            //
            if (!IsPostBack)
            {
                Grid1.SetSortPage(SiteConfig.Instance.PageSize);
                Grid1.BindGrid();
                UI.SetVisibleByQuery("search", Grid1.Toolbar);
            }
        }
    }
}
