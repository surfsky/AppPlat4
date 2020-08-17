using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Controls;
using App.Components;

namespace App.Pages
{
    [UI("用户部门")]
    [Auth(Powers.UserView, Powers.UserEdit, Powers.UserEdit, Powers.UserEdit)]
    [Param("userId", "用户ID", false)]
    [Param("deptId", "部门ID", false)]
    public partial class UserDeptForm : FormPage<UserDept>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                // userId, deptId
                var user = DAL.User.Get(Asp.GetQueryLong("userId"));
                var dept = DAL.Dept.Get(Asp.GetQueryLong("deptId"));
                if (user != null)
                {
                    UI.SetValue(this.pbUser, user, t => t.ID, t => t.NickName);
                    UI.SetEnable(false, pbUser);
                }
                if (dept != null)
                {
                    UI.SetValue(this.pbDept, dept, t => t.ID, t => t.Name);
                    UI.SetEnable(false, pbDept);
                }

                ShowForm();
            }
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.tbTitle, "");
            UI.SetValue(this.tbSeq, 0);
        }

        // 加载数据
        public override void ShowData(UserDept item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.pbUser, item.User, t => t.ID, t => t.NickName);
            UI.SetValue(this.pbDept, item.Dept, t => t.ID, t => t.Name);
            UI.SetValue(this.tbTitle, item.Title);
            UI.SetValue(this.tbSeq, item.Seq);
        }

        // 采集数据
        public override void CollectData(ref UserDept item)
        {
            item.UserID = UI.GetLong(this.pbUser);
            item.DeptID = UI.GetLong(this.pbDept);
            item.Title = UI.GetText(this.tbTitle);
            item.Seq = UI.GetInt(this.tbSeq);
        }
    }
}
