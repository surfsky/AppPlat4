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

namespace App.Admins
{
    [UI("签到")]
    [Auth(Powers.SignView, Powers.SignNew, Powers.SignEdit, Powers.SignDelete)]
    public partial class SignForm : FormPage<UserSign>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                ShowForm();
            }
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            this.lblId.Text = "-1";
            UI.SetValue(this.pbUser, Common.LoginUser, t => t.ID, t => t.NickName);
            UI.SetValue(this.dpSign, DateTime.Now);
            UI.SetValue(this.tbScore, 0);
        }

        // 加载数据
        public override void ShowData(UserSign item)
        {
            this.lblId.Text = item.ID.ToString();
            UI.SetValue(this.pbUser, item.User, t => t.ID, t => t.NickName);
            UI.SetValue(this.dpSign, item.SignDt);
            UI.SetValue(this.tbScore, item.Score);
        }

        // 采集数据
        public override void CollectData(ref UserSign item)
        {
            item.UserID = UI.GetLong(this.pbUser);
            item.SignDt = UI.GetDate(dpSign);
            item.Score = UI.GetInt(tbScore, 0);
            if (this.Mode == PageMode.New)
                item.SignDt = DateTime.Now;
        }

        // 保存
        public override void SaveData(UserSign item)
        {
            item.Save();
            UserScore.Add(ScoreType.Sign, item.UserID, item.Score.Value, item.UniID);
        }
    }
}
