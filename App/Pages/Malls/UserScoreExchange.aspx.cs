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
    [UI("用户积分兑换")]
    [Auth(Powers.ScoreNew)]
    [Param("userId", "用户ID")]
    public partial class UserScoreExchange : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowForm();
            }
        }


        // 显示
        public void ShowForm()
        {
            // userId
            var userId = Asp.GetQueryLong("userId");
            if (userId == null)
            {
                Asp.Fail("缺少 userId 参数");
                return;
            }
            var user = DAL.User.Get(userId);
            UI.SetValue(this.pbUser, user, t => t.ID, t => t.NickName);
            UI.SetValue(this.lblScore, user.FinanceScore);
            UI.SetValue(this.tbScore, "100");
        }


        // 保存
        protected void btnExchange_Click(object sender, EventArgs e)
        {
            var userID = UI.GetLong(this.pbUser);
            var score = UI.GetInt(this.tbScore, 0);
            var remark = UI.GetText(this.tbRemark);

            try
            {
                UserScore.Add(ScoreType.Exchange, userID, -score.Value, "", remark);
                var hideScript = ActiveWindow.GetHidePostBackReference();
                var info = string.Format("成功兑换积分 {0}", score);
                var alertScript = Alert.GetShowInTopReference(info, "积分兑换", hideScript);
                PageContext.RegisterStartupScript(alertScript + hideScript);
            }
            catch (Exception ex)
            {
                UI.ShowAlert(ex.Message);
            }
        }
    }
}
