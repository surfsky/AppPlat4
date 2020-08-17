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

namespace App.Admins
{
    [UI("组织管理")]
    [Auth(Powers.OrgView, Powers.OrgNew, Powers.OrgEdit, Powers.OrgDelete)]
    public partial class Orgs : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Grid1
                .SetPowers(this.Auth)
                .SetUrls("OrgForm.aspx")
                .AddColumn<Org>(t => t.Name, 150, "名称")
                .AddColumn<Org>(t => t.City, 150, "城市")
                .AddColumn<Org>(t => t.Addr, 200, "地址")
                .AddColumn<Org>(t => t.CertNo, 80, "证书编号")
                .AddThrumbnailColumn<Org>(t => t.CertPic, 40, "证书")
                .AddColumn<Org>(t => t.LegalPerson, 80, "法人")
                .AddColumn<Org>(t => t.LegalPersonIDCardNo, 80, "法人身份证编号")
                .AddThrumbnailColumn<Org>(t => t.LegalPersonIDCardPic, 40, "法人身份证")
                .AddColumn<Org>(t => t.LegalPersonTel, 80, "法人电话")
                .AddColumn<Org>(t => t.Approved, 80, "是否通过")
                .InitGrid<Org>(BindGrid, Grid1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                Grid1.SetSortPage<Org>(SiteConfig.Instance.PageSize, t=> t.Name);
                BindGrid();
            }
        }

        private void BindGrid()
        {
            Grid1.DataSource = Org.All;
            Grid1.DataBind();
        }
    }
}
