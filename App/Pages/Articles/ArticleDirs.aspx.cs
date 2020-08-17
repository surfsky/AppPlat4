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

/*
<Columns>
    <f:BoundField DataField = "Name" HeaderText="部门名称" DataSimulateTreeLevelField="TreeLevel" Width="150px" />
    <f:BoundField DataField = "Remark" HeaderText="部门描述" ExpandUnusedSpace="true" />
    <f:BoundField DataField = "Seq" HeaderText="排序" Width="80px" />
</Columns>
*/


namespace App.Admins
{
    [UI("文章目录管理")]
    [Auth(Powers.ArticleDirView, Powers.ArticleDirEdit, Powers.ArticleDirEdit, Powers.ArticleDirEdit)]
    public partial class ArticleDirs : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    "ArticleDirForm.aspx?md=new&parentid={0}",
                    "ArticleDirForm.aspx?md=view&id={0}",
                    "ArticleDirForm.aspx?md=edit&id={0}")
                .AddThrumbnailColumn<ArticleDir>(t => t.Icon, 40, "图标")
                .AddColumn<ArticleDir>(t => t.Name, 200, "名称", isTree: true)
                .AddColumn<ArticleDir>(t => t.Remark, 200, "备注")
                .AddColumn<ArticleDir>(t => t.Seq, 200, "排序")
                .AddColumn<ArticleDir>(t => t.FullName, 400, "全称")
                .InitGrid<ArticleDir>(BindGrid, Grid1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                Grid1.SetSortPage<ArticleDir>(80, t=>t.ID, allowSort: false, allowPage: false);
                BindGrid();
            }
        }

        //------------------------------------------
        // grid
        //------------------------------------------
        private void BindGrid()
        {
            Grid1.DataSource = ArticleDir.All;
            Grid1.DataBind();
        }
    }
}
