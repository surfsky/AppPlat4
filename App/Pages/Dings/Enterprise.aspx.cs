using System;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Components;

namespace App.Dings
{
    [Auth(Powers.Admin)]
    [UI("钉钉企业号接入测试")]
    public partial class Enterprise : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        // 显示Token
        protected void btnGetTokon_Click(object sender, EventArgs e)
        {
            var token = DingHelper.GetAccessToken();
            UI.ShowAlert(token.ToJson());
        }


        // 获取用户详细信息
        protected void btnGetUserInfo_Click(object sender, EventArgs e)
        {
            var authCode = UI.GetText(tbCode);
            var user = DingHelper.GetUser(authCode);
            var userInfo = DingHelper.GetUserInfo(user.Userid);
            UI.ShowAlert(userInfo.ToJson());
        }

        // 获取部门列表
        protected void btnGetDepts_Click(object sender, EventArgs e)
        {
            var rsp = DingHelper.GetDepartments("1");
            UI.ShowAlert(rsp.ToJson());
        }

        // 设置钉钉部门
        protected void btnSetDeptsFromDing_Click(object sender, EventArgs e)
        {
            var orgId = Common.CurrentOrg?.ID;
            var n = DAL.Dept.Clear();

            // 保存根目录
            var r = DingHelper.GetDepartment("1");
            var dept = new Dept();
            dept.ID = r.Id;
            dept.Name = r.Name;
            dept.ParentID = null;
            dept.OrgID = orgId;
            dept.Save();

            // 保存子目录
            var rsp = DingHelper.GetDepartments("1");
            foreach (var d in rsp.Department)
            {
                try
                {
                    dept = new Dept();
                    dept.ID = d.Id;
                    dept.Name = d.Name;
                    dept.ParentID = d.Parentid;
                    dept.OrgID = orgId;
                    dept.Save();
                }
                catch
                {
                    Logger.Log("Save fail", d.ToJson());
                }
            }
            Dept.LoadCache();
            UI.ShowAlert("共同步部门：" + rsp.Department.Count);
        }
    }
}
