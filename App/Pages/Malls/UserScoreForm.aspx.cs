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
    [UI("用户积分")]
    [Auth(Powers.ScoreView, Powers.ScoreNew, Powers.ScoreEdit, Powers.ScoreDelete)]
    [Param("userId", "用户ID")]
    public partial class UserScoreForm : FormPage<UserScore>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.BindEnum(ddlType, typeof(ScoreType));
                ShowForm();
            }
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            // userId
            var userId = Asp.GetQueryLong("userId");
            if (userId == null)
            {
                Asp.Fail("缺少 userId 参数");
                return;
            }
            var user = DAL.User.Get(userId);


            //
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.lblCreateDt, "");
            UI.SetValue(this.pbUser, user, t => t.ID, t => t.NickName);
            UI.SetValue(this.ddlType, ScoreType.Exchange);
            UI.SetValue(this.tbScore, "");
            UI.SetValue(this.tbRemark, "");
            UI.SetValue(this.tbSourecId, "");
        }

        // 加载数据
        public override void ShowData(UserScore item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.lblCreateDt, item.CreateDt);
            UI.SetValue(this.pbUser, item.User, t => t.ID, t => t.NickName);
            UI.SetValue(this.ddlType, item.Type);
            UI.SetValue(this.tbScore, item.Score);
            UI.SetValue(this.tbRemark, item.Remark);
            UI.SetValue(this.tbSourecId, item.SourceID);
        }

        // 采集数据
        public override void CollectData(ref UserScore item)
        {
            item.UserID = UI.GetLong(this.pbUser);
            item.Type = UI.GetEnum<ScoreType>(this.ddlType);
            item.Score = UI.GetInt(this.tbScore, 0);
            item.Remark = UI.GetText(this.tbRemark);
        }

        // 保存数据
        public override void SaveData(UserScore item)
        {
            item.Save();
            if (this.Mode == PageMode.New)
            {
                var user = DAL.User.Get(item.UserID);
                user.CalcScore(item.Score.Value);
            }
        }
    }
}
