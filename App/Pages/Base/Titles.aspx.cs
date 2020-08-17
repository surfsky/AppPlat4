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


/*
<f:BoundField DataField="Name" SortField="Name" Width="100px" HeaderText="职务名称" />
<f:BoundField DataField="Remark" ExpandUnusedSpace="true" HeaderText="备注" />
*/
namespace App.Admins
{
    [UI("职务管理")]
    [Auth(Powers.TitleView, Powers.TitleNew, Powers.TitleEdit, Powers.TitleDelete)]
    public partial class Titles : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("TitleForm.aspx")
                .AddColumn<Title>(t=>t.Name, 100, "职务")
                .AddColumn<Title>(t=>t.Remark, 200, "备注")
                .InitGrid<Title>(BindGrid, Panel1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<Title>(SiteConfig.Instance.PageSize, t => t.Name, true);
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var name = UI.GetText(tbName);
            var q = DAL.Title.Search(name);
            Grid1.Bind(q);
        }

        // 检索
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
