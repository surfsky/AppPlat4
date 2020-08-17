using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Linq;
using System.Data.Entity;
//using EntityFramework.Extensions;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Admins
{
    [UI("用户管理")]
    [Auth(Powers.UserView, Powers.UserNew, Powers.UserEdit, Powers.UserDelete)]
    public partial class Users : PageBase
    {
        //--------------------------------------------------
        // Init
        //--------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("UserForm.aspx")
                .AddWindowColumn<User>(t => t.ID, null, "UserPassword.aspx?id={0}", 38, "修改密码", "", Icon.Key, "ChangePassword")
                .AddThrumbnailColumn<User>(t => t.Avatar, 40, "照片")
                .AddColumn<User>(t => t.Name, 120, "用户名")
                .AddColumn<User>(t => t.NickName, 100, "昵称")
                .AddColumn<User>(t => t.RealName, 100, "真实姓名")
                .AddCheckColumn<User>(t => t.InUsed, 100, "启用")
                .AddColumn<User>(t => t.RoleNames, 300, "角色")  //
                .AddColumn<User>(t => t.Gender, 100, "性别")
                .AddColumn<User>(t => t.Mobile, 120, "手机")
                .AddColumn<User>(t => t.Tel, 100, "电话")
                .AddColumn<User>(t => t.Dept.Name, 100, "部门")
                //.AddColumn<User>(t => t.Titles, 100, "职务")  //
                //.AddWindowColumn<User>(t => t.ID, null, "UserDepts.aspx?userId={0}", 38, "管辖部门", "", Icon.Folder, "UserFolder")
                //.AddColumn<User>(t => t.Email, 100, "邮箱")
                //.AddColumn<User>(t => t.IDCard, 100, "身份证")
                //.AddColumn<User>(t => t.Birthday, 150, "生日")
                //.AddColumn<User>(t => t.HireDt, 150, "就职日期")
                //.AddColumn<User>(t => t.LastLoginDt, 150, "最后登录日期")
                //.AddColumn<User>(t => t.Remark, 100, "备注")
                .InitGrid<User>(BindGrid, Panel1, t => t.NickName)
                ;
            if (!IsPostBack)
            {
                //UI.BindTree(ddlDept, Dept.All, t => t.ID, t => t.Name, "--全部部门--", null, null);
                UI.Bind(ddlTitle, DAL.Title.Set.ToList(), t => t.ID, t => t.Name, "--全部职务--", null);
                UI.Bind(ddlRole, Role.All, nameof(Role.ID), nameof(Role.Name), "--全部角色--", null);
                UI.BindBool(ddlStatus, "启用", "禁用", "--全部状态--", true);
                UI.SetGridColumnByPower(Grid1, "ChangePassword", Powers.UserChangePassword);
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbMobile, this.tbName, this.pbDept, this.ddlTitle, this.ddlRole, this.ddlStatus);

                this.Grid1.SetSortPage<User>(SiteConfig.Instance.PageSize, t => t.Name, true);
                BindGrid();
            }
        }


        // 查询及绑定网格
        void BindGrid()
        {
            var name = UI.GetText(tbName);
            var mobile = UI.GetText(tbMobile);
            var deptId = UI.GetLong(pbDept);
            var titleId = UI.GetLong(ddlTitle);
            var roleId = UI.GetLong(ddlRole);
            var inUsed = UI.GetBool(ddlStatus);
            bool includeAdmin = Common.LoginUser.Name == "admin";
            var power = Asp.GetQuery<Powers>("power");

            if (power == null)
            {
                IQueryable<User> q = DAL.User.Search(
                    name: name,
                    mobile: mobile,
                    role: roleId,
                    inUsed: inUsed,
                    deptId: deptId,
                    titleId: titleId,
                    includeAdmin: includeAdmin
                    );
                Grid1.Bind(q);
            }
            else
            {
                var users = DAL.User.SearchByPower(power.Value);
                Grid1.Bind(users.AsQueryable());
            }
        }


        // 检索
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        // Grid 行显示事件（可以自由更改单元格内容）
        protected void Grid1_RowDataBound(object sender, GridRowEventArgs e)
        {
            User user = e.DataItem as User;

            // 设置角色
            //string roles = user.Roles.Select(t => t.GetTitle()).ToSeparatedString();
            //UI.SetGridCellText(Grid1, "Roles", roles, e);

            // 设置职务
            //string titles = user.Titles.Select(t => t.Name).ToSeparatedString();
            //UI.SetGridCellText(Grid1, "Titles", titles, e);

            // admin 账户禁止删除
            if (user.Name == "admin")
                UI.SetGridCellText(Grid1, "Delete", "", e);
        }
    }
}
