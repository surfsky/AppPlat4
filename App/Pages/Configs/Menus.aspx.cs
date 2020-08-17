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
using System.IO;
using App.Components;


/*
<f:BoundField DataField="ID" Width="50px"  HeaderText="ID" />
<f:ImageField DataImageUrlField="ImageUrl" Width="30px" />
<f:BoundField DataField="Name" HeaderText="菜单标题" DataSimulateTreeLevelField="TreeLevel" Width="250px" />
<f:BoundField DataField="NavigateUrl" HeaderText="链接" Width="250px" ExpandUnusedSpace="true"/>
<f:BoundField DataField="ViewPower" HeaderText="浏览权限" Width="120px" />
<f:BoundField DataField="Visible" HeaderText="可见" Width="60px" />
<f:BoundField DataField="IsOpen" HeaderText="展开" Width="60px" />
<f:BoundField DataField="Seq" HeaderText="排序" Width="60px" />
<f:BoundField DataField="Remark" HeaderText="备注" Width="100px" ExpandUnusedSpace="true" />
*/

namespace App.Admins
{
    [UI("菜单管理")]
    [Auth(Powers.Menu)]
    public partial class Menus : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    "MenuForm.aspx?md=new&parentId={0}",
                    "MenuForm.aspx?md=view&id={0}",
                    "MenuForm.aspx?md=edit&id={0}"
                    )
                .AddColumn<DAL.Menu>(t=>t.ID, 50, "ID")
                .AddImageColumn<DAL.Menu>(t => t.ImageUrl, 30)
                .AddColumn<DAL.Menu>(t => t.Name, 250, "标题", isTree: true)
                .AddColumn<DAL.Menu>(t => t.NavigateUrl, 250, "链接")
                .AddColumn<DAL.Menu>(t => t.ViewPower, 120, "浏览权限")
                .AddCheckColumn<DAL.Menu>(t => t.Visible, 60, "可见")
                .AddCheckColumn<DAL.Menu>(t => t.IsOpen, 60, "展开")
                .AddColumn<DAL.Menu>(t => t.Seq, 60, "排序")
                .AddColumn<DAL.Menu>(t => t.Remark, 100, "备注")
                .InitGrid<DAL.Menu>(BindGrid, Panel1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                Grid1.SetSortPage<DAL.Menu>(SiteConfig.Instance.PageSize, t => t.ID, allowSort: false, allowPage: false);
                BindGrid();
            }
        }

        // 数据绑定
        private void BindGrid()
        {
            var menus = chkError.Checked ? GetErrorMenus() : DAL.Menu.All;
            Grid1.DataSource = menus;
            Grid1.DataBind();
        }

        /// <summary>找出有问题的菜单</summary>
        public static List<DAL.Menu> GetErrorMenus()
        {
            var errors = new List<DAL.Menu>();
            foreach (var menu in DAL.Menu.All)
            {
                if (menu.NavigateUrl.IsEmpty())
                    continue;
                var url = menu.NavigateUrl.ToLower().TrimQuery();
                if (url.StartsWith("http:"))
                    continue;
                if (!File.Exists(Asp.MapPath(url)))
                    errors.Add(menu);
            }
            return errors;
        }

        //----------------------------------------------------
        // 事件
        //----------------------------------------------------
        /// <summary>错误单选框</summary>
        protected void chkError_CheckedChanged(object sender, CheckedEventArgs e)
        {
            BindGrid();
        }

        // 调整排序
        protected void Grid1_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.StartsWith("Move"))
            {
                object[] keys = Grid1.DataKeys[e.RowIndex];
                var id = Convert.ToInt64(keys[0]);
                var menu = DAL.Menu.All.Find(t => t.ID == id);
                if      (e.CommandName == "MoveUp")     menu.MoveUp();
                else if (e.CommandName == "MoveDown")   menu.MoveDown();
                else if (e.CommandName == "MoveTop")    menu.MoveTop();
                else if (e.CommandName == "MoveBottom") menu.MoveBottom();
                Common.LoginUser.SetMenus();
                BindGrid();
            }
        }
    }
}
