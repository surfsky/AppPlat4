using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Linq;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Controls;
using App.Components;

namespace App.Admins
{
    [UI("管理员协助修改用户密码")]
    [Auth(Powers.UserChangePassword)]
    public partial class UserPassword : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnClose.OnClientClick = ActiveWindow.GetHideReference();
                LoadData();
            }
        }

        // 读取用户信息
        private void LoadData()
        {
            var id = Asp.GetQueryLong("id");
            if (id == null)
            {
                Asp.Fail("参数错误");
                return;
            }
            User user = DAL.User.Get(id);
            if (user == null)
            {
                Asp.Fail("用户不存在");
                //Alert.Show("用户不存在！", String.Empty, ActiveWindow.GetHideReference());
                return;
            }
            if (user.Name == "admin" && AuthHelper.GetLoginUserName() != "admin")
            {
                Asp.Fail("你无权编辑超级管理员");
                //Alert.Show("你无权编辑超级管理员！", String.Empty, ActiveWindow.GetHideReference());
                return;
            }
            lblUserName.Text = user.Name;
            lblNickName.Text = user.NickName;
        }

        // 保存并关闭
        protected void btnSaveClose_Click(object sender, EventArgs e)
        {
            var id = Asp.GetQueryLong("id").Value;
            var password = this.tbPassword.Text.Trim();
            var password2 = this.tbConfirmPassword.Text.Trim();
            if (password != password2)
            {
                UI.ShowAlert("密码不一致");
                return;
            }

            User item = DAL.User.Get(id);
            DAL.User.SetPassword(item,password );
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        // 设置默认密码
        protected void btnSet_Click(object sender, EventArgs e)
        {
            var password = SiteConfig.Instance.DefaultPassword;
            this.tbPassword.Text = password;
            this.tbConfirmPassword.Text = password;
        }
    }
}
