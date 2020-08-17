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
    [UI("开放平台-组织管理")]
    [Auth(Powers.Admin)]
    public partial class Orgs : MixPage<Org>
    {
        protected override UISetting GetGridUI()
        {
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
            return ui;
        }

        protected override UISetting GetFormUI()
        {
            var ui = new UISetting<Org>(true);
            ui.SetEditorImage(t => t.CertPic, new System.Drawing.Size(500, 500));
            ui.SetEditorImage(t => t.LegalPersonIDCardPic, new System.Drawing.Size(500, 500));
            ui.SetMode(t => t.Approved, PageMode.Edit);
            ui.SetMode(t => t.InUsed, PageMode.Edit);
            return ui;
        }
    }
}
