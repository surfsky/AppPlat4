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
    [UI("用户积分管理")]
    [Auth(Powers.ScoreView, Powers.ScoreNew, Powers.ScoreEdit, Powers.ScoreDelete)]
    public partial class UserScores : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .AddColumn<UserScore>(t => t.TypeName, 200, "类型")
                .AddColumn<UserScore>(t => t.User.NickName, 200, "用户")
                .AddColumn<UserScore>(t => t.Score, 200, "分数")
                .AddColumn<UserScore>(t => t.CreateDt, 200, "时间")
                .AddColumn<UserScore>(t => t.Remark, 400, "备注")
                .InitGrid<UserScore>(this.BindGrid, Panel1, t => t.CreateDt)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<UserScore>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbUser, this.dpCreate, this.btnSearch);
            }
        }


        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        private void BindGrid()
        {
            var userId = Asp.GetQueryLong("userId");
            var user = UI.GetText(tbUser);
            var createDt = UI.GetDate(this.dpCreate);
            IQueryable<UserScore> q = UserScore.Search(
                userId: userId,
                userName:user, 
                startDt: createDt
                );
            Grid1.Bind(q);
        }

        //-------------------------------------------------
        // 工具栏
        //-------------------------------------------------
        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        // 积分兑换
        protected void btnExchange_Click(object sender, EventArgs e)
        {
            var url = string.Format("UserScoreExchange.aspx?userId={0}", Asp.GetQueryLong("userId"));
            var script = this.Grid1.Win.GetShowReference(url);
            PageContext.RegisterStartupScript(script);
        }
    }
}
