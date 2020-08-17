using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Linq;
using App.DAL;
using App.Controls;
using App.Components;
using App.Utils;

namespace App.Admins
{
    [UI("用户修改自己的密码")]
    [Auth(AuthLogin =true)]
    public partial class Profiles : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        // 保存密码
        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            // 检查密码一致性
            string oldPassword = UI.GetText(tbOldPassword);
            string newPassword = UI.GetText(tbNewPassword);
            string confirmPassword = UI.GetText(tbConfirmPassword);
            if (newPassword != confirmPassword)
            {
                tbConfirmPassword.MarkInvalid("确认密码和新密码不一致！");
                return;
            }

            var user = DAL.User.Get(Common.LoginUser.ID);
            if (!PasswordHelper.Compare(user.Password, oldPassword))
                tbOldPassword.MarkInvalid("旧密码不正确！");
            else
            {
                user.Password = PasswordHelper.CreateDbPassword(newPassword);
                user.Save();
                Alert.ShowInTop("密码修改成功");
            }
        }
    }
}
