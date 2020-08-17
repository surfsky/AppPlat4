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
    [UI("组织管理")]
    [Auth(Powers.OrgView)]
    public partial class OrgForm : PageBase
    {
        // 初始化
        protected void Page_Load(object sender, EventArgs e)
        {
            var ui = new UISetting<Org>(true);
            ui.SetEditorImage(t => t.CertPic, new System.Drawing.Size(500, 500));
            ui.SetEditorImage(t => t.LegalPersonIDCardPic, new System.Drawing.Size(500, 500));
            ui.SetMode(t => t.Approved, PageMode.Edit);
            ui.SetMode(t => t.InUsed, PageMode.Edit);
            this.form2.ShowIdLabel = false;
            this.form2.Build(ui);

        }
    }
}
