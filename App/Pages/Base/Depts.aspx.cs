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
    [UI("部门管理")]
    [Auth(Powers.DeptView, Powers.DeptNew, Powers.DeptEdit, Powers.DeptDelete)]
    public partial class Depts : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    "DeptForm.aspx?md=new&parentid={0}",
                    "DeptForm.aspx?md=view&id={0}",
                    "DeptForm.aspx?md=edit&id={0}")
                .AddColumn<Dept>(t => t.Name, 200, "名称", isTree: true)
                //.AddColumn<Dept>(t => t.DingDeptID, 200, "钉钉部门ID")
                .AddColumn<Dept>(t => t.Remark, 200, "备注")
                .AddColumn<Dept>(t => t.Seq, 200, "排序")
                .InitGrid<Dept>(BindGrid, Grid1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                Grid1.SetSortPage<Dept>(SiteConfig.Instance.PageSize, null, allowSort: false, allowPage: false);
                BindGrid();
            }
        }

        //------------------------------------------
        // grid
        //------------------------------------------
        private void BindGrid()
        {
            Grid1.DataSource = Dept.All;
            Grid1.DataBind();
        }
    }
}
