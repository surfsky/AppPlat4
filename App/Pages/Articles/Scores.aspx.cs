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
    [UI("积分管理")]
    [Auth(Powers.UserScoreView)]
    public partial class Scores : PageBase
    {
        //--------------------------------------------------
        // Init
        //--------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(false, false, false, false)
                //.SetUrls("UserForm.aspx")
                .AddColumn<User>(t => t.Name, 120, "用户名")
                .AddColumn<User>(t => t.RealName, 100, "真实姓名")
                .AddColumn<User>(t => t.Dept.Name, 100, "部门")
                .AddColumn<User>(t => t.Remark, 100, "积分")
                .InitGrid<User>(BindGrid, Panel1, t => t.RealName)
                ;
            if (!IsPostBack)
            {
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbName, this.pbDept, this.dpStart, this.dpEnd);
                this.Grid1.SetSortPage<User>(SiteConfig.Instance.PageSize, t => t.Name, true);
                BindGrid();
            }
        }


        // 查询及绑定网格
        void BindGrid()
        {
            var name = UI.GetText(tbName);
            var deptId = UI.GetLong(pbDept);


            IQueryable<User> q = DAL.User.Search(
                name: name,
                deptId: deptId
                );
            Grid1.Bind(q);

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

            var startDt = UI.GetDate(this.dpStart);
            var endDt = UI.GetDate(this.dpEnd);

            // 设置积分
            string scores = App.Components.Score.GetNum(user.ID, startDt, endDt).ToString();
            UI.SetGridCellText(Grid1, "Remark", scores, e);

            ////设置职务
            //string titles = user.Titles.Select(t => t.Name).ToSeparatedString();
            //UI.SetGridCellText(Grid1, "Titles", titles, e);

            //// admin 账户禁止删除
            //if (user.Name == "admin")
            //    UI.SetGridCellText(Grid1, "Delete", "", e);
        }
    }
}
