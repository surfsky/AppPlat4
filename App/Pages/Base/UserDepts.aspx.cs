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
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Pages
{
    [UI("用户管辖部门")]
    [Auth(Powers.UserView, Powers.UserNew, Powers.UserEdit, Powers.UserDelete)]
    [Param("userId", "用户ID")]
    [Param("deptId", "部门ID")]
    public partial class UserDepts : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Asp.GetQueryLong("userId");
            var deptId = Asp.GetQueryLong("deptId");
            var url = $"UserDeptForm.aspx?userId={userId}&deptId={deptId}";
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(url)
                .AddColumn<UserDept>(t => t.User.NickName, 200, "用户")
                .AddColumn<UserDept>(t => t.Dept.Name, 200, "部门")
                .AddColumn<UserDept>(t => t.Title, 200, "头衔")
                .AddColumn<UserDept>(t => t.Seq, 80, "排名")
                .AddColumn<UserDept>(t => t.CreateDt, 200, "创建时间")
                .AddColumn<UserDept>(t => t.UpdateDt, 200, "修改时间")
                .InitGrid<UserDept>(this.BindGrid, Panel1, t => t.UserID)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<UserDept>(SiteConfig.Instance.PageSize, t => t.DeptID);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbUser, this.tbDept, this.btnSearch);
                UI.SetVisible(userId == null, this.tbUser, this.tbDept, this.btnSearch);
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var userId = Asp.GetQueryLong("userId");
            var user = UI.GetText(tbUser);
            var dept = UI.GetText(tbDept);
            IQueryable<UserDept> q = UserDept.Search(
                userId:   userId,
                userName: user,
                deptName: dept
                );
            Grid1.Bind(q);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
