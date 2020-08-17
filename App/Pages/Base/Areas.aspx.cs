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
    [UI("区域管理")]
    [Auth(Powers.AreaView, Powers.AreaNew, Powers.AreaEdit, Powers.AreaDelete)]
    public partial class Areas : PageBase
    {
        /*
        <f:BoundField DataField="Name" HeaderText="名称" DataSimulateTreeLevelField="TreeLevel" Width="150px" />
        <f:BoundField DataField="TypeName" HeaderText="类别" Width="150px" />
        <f:BoundField DataField="FullName" HeaderText="全称" Width="200" ExpandUnusedSpace="true" />
        <f:BoundField DataField="Remark" HeaderText="描述" ExpandUnusedSpace="true" />
        <f:BoundField DataField="Seq" HeaderText="排序" Width="80px" />
        */
        protected void Page_Load(object sender, EventArgs e)
        {
            Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    "AreaForm.aspx?md=new&parentid={0}",
                    "AreaForm.aspx?md=view&id={0}",
                    "AreaForm.aspx?md=edit&id={0}"
                )
                .AddColumn<Area>(t => t.Name, 150, "名称", isTree:true)
                .AddColumn<Area>(t => t.TypeName, 150, "类别")
                .AddColumn<Area>(t => t.FullName, 200, "全称")
                .AddColumn<Area>(t => t.Seq, 80, "排序")
                .AddColumn<Area>(t => t.Remark, 200, "描述")
                .InitGrid<Area>(BindGrid, Grid1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                Grid1.SetSortPage<Area>(SiteConfig.Instance.PageSize, t=> t.ID, allowSort: false, allowPage: false);
                BindGrid();
            }
        }

        //------------------------------------------
        // grid
        //------------------------------------------
        private void BindGrid()
        {
            Grid1.DataSource = Area.All;
            Grid1.DataBind();
        }
    }
}
