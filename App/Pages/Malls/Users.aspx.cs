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

namespace App.Pages
{
    [UI("用户管理（商城）")]
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
                .InitGrid<User>(BindGrid, Panel1, t => t.NickName, true)
                ;
            if (!IsPostBack)
            {
                UI.Bind(ddlRole, Role.All, nameof(Role.ID), nameof(Role.Name), "--全部角色--", null);
                UI.BindBool(ddlStatus, "启用", "禁用", "--全部状态--", true);
                UI.SetGridColumnByPower(Grid1, "changePasswordField", Powers.UserChangePassword);
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbMobile, this.tbName, this.ddlRole, this.ddlStatus, this.pbShop);
                MallHelper.LimitShop(this.pbShop);


                // 显示数据
                this.Grid1.SetSortPage<User>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
            }
        }

        // 查询及绑定网格
        void BindGrid()
        {
            IQueryable<User> q = Query();
            Grid1.Bind(q);
        }
        private IQueryable<User> Query()
        {
            var name = UI.GetText(tbName);
            var mobile = UI.GetText(tbMobile);
            var shopId = UI.GetLong(pbShop);
            var roleId = UI.GetLong(ddlRole);
            var inUsed = UI.GetBool(ddlStatus);
            bool includeAdmin = Common.LoginUser.Name == "admin";

            IQueryable<User> q = DAL.User.Search(
                name: name,
                mobile: mobile,
                role: roleId,
                inUsed: inUsed,
                shopId: shopId,
                includeAdmin: includeAdmin
                );
            return q;
        }



        //--------------------------------------------------
        // 工具栏
        //--------------------------------------------------
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.BindGrid();
        }


        //--------------------------------------------------
        // Grid
        //--------------------------------------------------
        // 导出
        protected void Grid1_Export(object sender, EventArgs e)
        {
            var list = Query().OrderBy(t => t.CreateDt).ToList().Select(t => new
            {
                t.ID,
                用户名 = t.Name,
                真名 = t.RealName,
                昵称 = t.NickName,
                状态 = t.InUsed,
                角色 = t.RoleNames,
                手机 = t.Mobile,
                注册时间 = t.CreateDt,
                连续签到天数 = t.ContinueSignDays,
                积分 = t.FinanceScore,
                余额 = t.FinanceBalance,
                性别 = t.Gender,
                生日 = t.Birthday,
                年龄 = t.Age,
                地址 = t.Address,
                Email = t.Email,
            }).ToList();
            this.Grid1.ExportExcel(list, "用户.xls");
        }
    }
}
